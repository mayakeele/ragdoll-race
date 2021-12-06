using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    [Header("Component References")]
    [SerializeField] private ControllerInputs inputSystem;
    [SerializeField] private Player player;


    [Header("Ground Movement Settings")]
    [SerializeField] private float groundSpeed;
    [Space]
    [SerializeField] private float groundAcceleration;
    [SerializeField] private float groundDeceleration;
    [Space]
    [SerializeField] private float groundTurnMinAngle;
    

    [Header("Air Movement Settings")]
    [SerializeField] private float airSpeed;
    [Space]
    [SerializeField] private float airAcceleration;
    [SerializeField] private float airDeceleration;
    [Space]
    [SerializeField] private float airDecelerationNoInput;
    [Space]
    [SerializeField] private float airTurnMinAngle;
 
    
    [Header("Jump Settings")]
    [SerializeField] private int numAirJumpsTotal;
    [SerializeField] private float jumpSpeedGrounded;
    [SerializeField] private float jumpSpeedAir;
    [SerializeField] private float jumpSpringDisableTime;
    [SerializeField] private int jumpPhysicsFrames;


    [Header("Turning Settings")]
    [SerializeField] private float turnSpringConstant;
    [SerializeField] private float turnDampingConstant;


    [Header("Threshold Settings")]
    [SerializeField] private float moveInputStickThreshold;
    [SerializeField] private float rotateInputStickThreshold;
    [SerializeField] private float ragdollGetupStickThreshold;



    // Input Variables
    Vector2 moveInput = Vector2.zero;
    Vector2 rotateInput = Vector2.zero;
    bool jumpInput = false;


    // Movement Variables
    public float currMoveSpeedLimit;
    public float currMoveAcceleration;

    public bool isDecelerating;

    private Vector3 currLookDirection = Vector3.one;

    private int numAirJumpsRemaining;


    // Main Functions

    void Awake(){
        inputSystem = new ControllerInputs();
    }

    void Start()
    {
        currMoveSpeedLimit = groundSpeed;
        currMoveAcceleration = groundAcceleration;
    }


    void FixedUpdate()
    {
        // Only allow player to move if not in ragdoll state
        if(!player.isRagdoll){

            // Set current speed limit
            currMoveSpeedLimit = SetIdealSpeed();


            // Get camera space
            Vector3 cameraForward = player.manager.cameraController.GetCameraForwardDirection();
            Vector3 cameraRight = Vector3.Cross(Vector3.up, cameraForward);


            // Calculate the ideal velocity both relative to the ground and in world space
            Vector3 idealVelocityRelative = ((cameraForward * moveInput.y) + (cameraRight * moveInput.x)) * currMoveSpeedLimit;
            Vector3 idealVelocityWorld = idealVelocityRelative + player.groundVelocity.ProjectHorizontal();
            Vector3 currentVelocityWorld = player.rootRigidbody.velocity.ProjectHorizontal();

            // Determine whether the player is accelerating or decelerating based on the angle between velocity and acceleration
            Vector3 requiredVelocityChange = (idealVelocityWorld - currentVelocityWorld);
            
            // Set current acceleration value and delta v
            currMoveAcceleration = SetIdealAcceleration(currentVelocityWorld, requiredVelocityChange);
            float perFrameSpeedChange = currMoveAcceleration * Time.fixedDeltaTime;


            // Calculate force required to move towards or exactly equal the target velocity without overshoot
            Vector3 movementForce = Vector3.zero;
            if(requiredVelocityChange.magnitude > perFrameSpeedChange){
                movementForce = player.activeRagdoll.GetBodyMass() * currMoveAcceleration * requiredVelocityChange.normalized;
            }
            else{
                movementForce = player.activeRagdoll.GetBodyMass() * requiredVelocityChange / Time.fixedDeltaTime;
            }

            // Apply movement force to player, and if applicable, the rigidbody they are standing on
            player.rootRigidbody.AddForce(player.activeRagdoll.GetBodyMass() * currMoveAcceleration * requiredVelocityChange.normalized);
            if(player.groundRigidbody){
                player.groundRigidbody.AddForceAtPosition(-movementForce, player.groundPosition);
            }


            // Determine where the character should rotate, then turn towards that
            if(rotateInput.magnitude > rotateInputStickThreshold){
                currLookDirection = rotateInput.ProjectHorizontalNormalized();
            }
            else{
                currLookDirection = idealVelocityRelative.ProjectHorizontal();
            }


            TurnTowardsDirection(currLookDirection);
        }   
    }


    void OnEnable(){
        inputSystem.Gameplay.Enable();
    }
    void OnDisable(){
        inputSystem.Gameplay.Disable();
    }



    // Input Events

    public void OnMoveInput(InputAction.CallbackContext context){
        moveInput = context.action.ReadValue<Vector2>();

        // If the stick input exceeds a threshold, try to make player stand upright if they are ragdolled
        if(player.isRagdoll && moveInput.magnitude > ragdollGetupStickThreshold){
            player.TrySetRagdollState(false);
        }
    }

    public void OnRotateInput(InputAction.CallbackContext context){
        rotateInput = context.action.ReadValue<Vector2>();

        // If the stick input exceeds a threshold, try to make player stand upright if they are ragdolled
        if(player.isRagdoll && rotateInput.magnitude > ragdollGetupStickThreshold){
            player.TrySetRagdollState(false);
        }
    }

    public void OnJumpInput(InputAction.CallbackContext context){

        // Try to make player stand upright if they are ragdolled

        //jumpInput = context.action.triggered;
        jumpInput = context.started;

        player.TrySetRagdollState(false);

        if(jumpInput && !player.isRagdoll){
            if(player.isGrounded){
                JumpGrounded();
            }
            else{
                if(numAirJumpsRemaining > 0){
                    numAirJumpsRemaining--;
                    JumpAir();
                }             
            }
        }
    }

    public void OnToggleRagdollInput(InputAction.CallbackContext context){
        // Tell the player script that the user wants to toggle ragdoll mode
        if(context.started){
            player.TrySetRagdollState(!player.isRagdoll);
        }
    }

    public void OnArmActionInput(InputAction.CallbackContext context){
        // Tell the Arms Action Coordinator that the user wants to use the left arm
        if(context.started){
            player.activeRagdoll.armsActionCoordinator.OnArmActionButtonPressed();
        }
        else if(context.canceled){
            player.activeRagdoll.armsActionCoordinator.OnArmActionButtonReleased();
        }
    }



    public void ResetAirJumpCount(){
        numAirJumpsRemaining = numAirJumpsTotal;
    }
    


    // Private Functions

    private float SetIdealSpeed(){
        // Updates the ideal player speed based on player condition and inputs
        
        float speed = 0;

        // Grounded movement
        if(player.isGrounded){          
            speed = groundSpeed;
            
        }
        // Air movement
        else{       
            speed = airSpeed; 
        }

        return speed;
    }

    private float SetIdealAcceleration(Vector3 currentVelocity, Vector3 deltaVelocity){
        // Sets the ideal acceleration value based on the player condition and inputs

        float accelerationAngle = Vector3.Angle(currentVelocity, deltaVelocity);

        float accel = 0;
        
        // Ground movement
        if(player.isGrounded){

            // Continuing in same direction
            if(accelerationAngle < groundTurnMinAngle){
                accel = groundAcceleration;
            }
            
            // Sharp turn
            else{
                accel = groundDeceleration;
            }  
        }

        // In the air, only accelerate if the player specifically inputs it. Otherwise continue with the same velocity
        else{    
            
            // Input is below threshold
            if(moveInput.magnitude < moveInputStickThreshold){
                accel = airDecelerationNoInput;
            }

            // Player is inputting a direction
            else{
                // Forward
                if(accelerationAngle < airTurnMinAngle){
                    accel = airAcceleration;
                }
                // Sharp turn
                else{
                    accel = airDeceleration;
                }
            }
        }

        return accel;
    }


    private void TurnTowardsDirection(Vector3 targetDirection){
        // Applies a torque to the root rigidbody around the vertical axis to face a given direction

        Vector3 currDirection = player.rootForward.transform.forward.ProjectHorizontal();
        Vector3 turningTorque = DampedSpring.GetDampedSpringTorque(currDirection, targetDirection, player.rootRigidbody.angularVelocity, turnSpringConstant, turnDampingConstant);

        player.rootRigidbody.AddTorque(turningTorque);
    }


    private void JumpGrounded(){
        StartCoroutine(player.activeRagdoll.PerformJump(jumpSpeedGrounded, jumpPhysicsFrames, jumpSpringDisableTime));
    }

    private void JumpAir(){

        float yVelocity = player.activeRagdoll.GetRelativeVelocity().y;

        // If player is falling or moving up, cancel fall and set vertical velocity to jump speed
        if(yVelocity < jumpSpeedAir){

            StartCoroutine(player.activeRagdoll.PerformJump(jumpSpeedAir - yVelocity, jumpPhysicsFrames, 0));
        }
        // If moving up any faster, don't do anything
        else{
            
        }


        // Spawns ring VFX below the player's feet
        Vector3 averageFeetPosition = player.activeRagdoll.GetAverageFeetPosition();
        Vector3 ringPosition = new Vector3(averageFeetPosition.x, player.activeRagdoll.GetLowerFootPosition().y, averageFeetPosition.z);
        player.vfx.SpawnAirJumpVFX(ringPosition);
    }
}
