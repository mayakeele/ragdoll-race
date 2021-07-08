using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketRidePowerup : Powerup
{
    private enum LifetimePhase{
        Inactive,
        Spawning,
        Chargeup,
        Moving
    }


    [Header("References")]
    [SerializeField] private GameObject rocketPrefab;


    [Header("Rocket Spawn")]
    [SerializeField] private float playerAngleThreshold;
    [SerializeField] private float playerShiftDistance;
    [SerializeField] private float playerShiftDuration;
    [SerializeField] private AnimationCurve playerShiftCurve;


    [Header("Rocket Chargeup")]
    [SerializeField] private float rocketChargeupDuration;


    [Header("Player Input")]
    [SerializeField] private float turningDeadzone;
    [Space]
    [SerializeField] private float directionalInputForwardSpeed;
    [SerializeField] private float directionalInputBackwardSpeed;
    [SerializeField] private float directionalInputSidewaysSpeed;
    [Space]
    [SerializeField] private float jumpOffSpeed;



    private Rigidbody playerPelvisRigidbody;
    private GameObject rocketObject;
    private RocketRideEntity rocketEntity;
    private Joint rocketJoint;

    private LifetimePhase currentPhase = LifetimePhase.Inactive;

    private Vector3 movementInputWorld = Vector3.zero;



    void FixedUpdate(){
        // Handle player directional input by rotating and moving the rocket

        if(rocketEntity && rocketEntity.isPlayerAttached){
            // Allow for free steering during chargeup phase
            if(currentPhase == LifetimePhase.Chargeup){
                if(movementInputWorld.magnitude > turningDeadzone){
                    rocketEntity.RotateRocketDirection(movementInputWorld);
                }
            }

            // Limited directional speed input while moving
            else if(currentPhase == LifetimePhase.Moving){
                Vector3 movementInputRocketSpace = rocketObject.transform.InverseTransformDirection(movementInputWorld);

                float forwardBackwardInputSpeed = movementInputRocketSpace.z > 0 ? directionalInputForwardSpeed : directionalInputBackwardSpeed;
                Vector2 velocityInputRocketSpace = new Vector2(directionalInputSidewaysSpeed * movementInputRocketSpace.x, forwardBackwardInputSpeed * movementInputRocketSpace.z);

                rocketEntity.MoveRocket(velocityInputRocketSpace);
            }
        }
    }



    public override bool OnActivateInitial()
    {
        playerPelvisRigidbody = attachedPowerupManager.player.rootRigidbody;

        // Only activate the powerup if the player is upright and not ragdolled

        float playerUprightAngle = Vector3.Angle(attachedPowerupManager.player.rootForward.up, Vector3.up);
        if(playerUprightAngle <= playerAngleThreshold && !attachedPowerupManager.player.isRagdoll){

            currentPhase = LifetimePhase.Spawning;

            // Spawn the rocket
            Vector3 spawnPosition = attachedPowerupManager.player.rootRigidbody.position;
            Vector3 forwardDirection = attachedPowerupManager.player.rootForward.forward.ProjectHorizontal();
            Quaternion spawnRotation = Quaternion.LookRotation(forwardDirection, Vector3.up);
            rocketObject = SpawnedEntity.SpawnEntityForPlayer(rocketPrefab, attachedPowerupManager.player, spawnPosition, spawnRotation);

            // Begin scaling the rocket's model and collider from 0 to full scale
            rocketEntity = rocketObject.GetComponent<RocketRideEntity>();
            rocketEntity.powerup = this;
            StartCoroutine(rocketEntity.ScaleModel(playerShiftDuration));

            // Begin shifting the player upwards
            StartCoroutine(ShiftPlayerUpwards());

            return true;
        }
        else{
            return false;
        }
        
    }


    public override void OnActivateContinued()
    {
        if(rocketEntity && rocketEntity.isPlayerAttached && currentPhase == LifetimePhase.Moving){
            rocketEntity.ExplodeRocket();
        }
    }


    public override void OnInputJump()
    {
        // Detaches from the rocket if in moving phase, and the player isn't already attached

        if(rocketEntity && rocketEntity.isPlayerAttached && (currentPhase == LifetimePhase.Moving || currentPhase == LifetimePhase.Chargeup)){
            attachedPowerupManager.RemovePowerup(this);
            JumpOffRocket(jumpOffSpeed);
        }
    }


    public override void OnInputMove(Vector2 movementInput)
    {
        // Calculate and store the world-space direction of the player's input
        Vector3 cameraForward = attachedPowerupManager.player.manager.cameraController.GetCameraForwardDirection();

        Vector3 worldForward = movementInput.y * cameraForward;
        Vector3 worldRight = movementInput.x * Vector3.Cross(Vector3.up, cameraForward);

        movementInputWorld = worldForward + worldRight;
    }


    public override void OnRemove()
    {
        if(rocketEntity) rocketEntity.isPlayerAttached = false;
        if(rocketJoint) Destroy(rocketJoint);

        if(playerPelvisRigidbody) playerPelvisRigidbody.isKinematic = false;
    }



    public void OnExplosion(){
        JumpOffRocket(jumpOffSpeed);
        attachedPowerupManager.RemovePowerup(this);   
    }



    private IEnumerator ShiftPlayerUpwards(){
        // Makes the player's pelvis rigidbody kinematic and shifts it upwards over time
        // When finished, creates a joint between the rocket and the player

        playerPelvisRigidbody.isKinematic = true;

        Vector3 initialPosition = playerPelvisRigidbody.position;
        float currentTime = 0;

        // Move the pelvis rigidbody upwards over time
        while(currentTime < playerShiftDuration){
            float timeGradient = currentTime / playerShiftDuration;
            float distanceGradient = playerShiftCurve.Evaluate(timeGradient);

            Vector3 displacement = distanceGradient * playerShiftDistance * Vector3.up;

            playerPelvisRigidbody.MovePosition(initialPosition + displacement);

            currentTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // Finalize the player's position
        playerPelvisRigidbody.position = initialPosition + (playerShiftDistance * Vector3.up);
        playerPelvisRigidbody.isKinematic = false;

        // Create joint between rocket and player pelvis
        rocketJoint = rocketObject.AddComponent<FixedJoint>();
        rocketJoint.connectedBody = playerPelvisRigidbody;


        // Set up the chargeup phase
        StartCoroutine(PerformChargeup());
    }
    

    private IEnumerator PerformChargeup(){

        currentPhase = LifetimePhase.Chargeup;
        rocketEntity.canExplode = true;

        // ~~~~~~~ TODO special effects here ~~~~~~~~~~

        yield return new WaitForSeconds(rocketChargeupDuration);

        currentPhase = LifetimePhase.Moving;
    }


    public void JumpOffRocket(float jumpSpeed){
        float bodyMass = attachedPowerupManager.player.activeRagdoll.GetBodyMass();
        Vector3 playerVelocity = attachedPowerupManager.player.rootRigidbody.velocity;

        Vector3 jumpForce = (bodyMass / Time.fixedDeltaTime) * jumpSpeed * Vector3.up;
        Vector3 backwardsForce = (bodyMass / Time.fixedDeltaTime) * -playerVelocity.ProjectHorizontal();

        playerPelvisRigidbody.AddForce(jumpForce + backwardsForce);
    }

}
