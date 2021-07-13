using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarpoonHookEntity : SpawnedEntity
{
    [Header("References")]
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private SphereCollider hitTrigger;
    [HideInInspector] public HarpoonPowerup attachedPowerup;


    [Header("Launch Properties")]
    [SerializeField] private float launchSpeedInitial;
    [SerializeField] private float straightTravelDistance;
    [SerializeField] private float dragAcceleration;
    [SerializeField] private float gravityAcceleration;


    [Header("Collision Properties")]
    [SerializeField] private LayerMask hookableLayers;
    [Space]
    [SerializeField] private Transform tipTransform;
    [SerializeField] private Transform ringTransform;


    [Header("Hit Properties")]
    [SerializeField] private float hitDamage;
    [SerializeField] private float hitKnockbackMultiplier;
    


    bool isHookable = true;

    private Vector3 currentVelocity = Vector3.zero;
    private float distanceCovered = 0;

    private FixedJoint joint;
    


    void OnTriggerEnter(Collider other)
    {
        // If the hook is active (isHookable), determine whether the hit was successful or not

        Hittable hittable = other.gameObject.GetComponent<Hittable>();

        if(isHookable && hittable.player != GetAttachedPlayer()){
            if(hookableLayers.ContainsLayer(other.gameObject.layer)){
                OnHookSuccess(other.transform);
            }
            else{
                OnHookFailure();
            }

        }   
    }

    public void OnHookSuccess(Transform hookedTransform){
        // Deactivates collision and attaches itself to the hit transform with a fixed joint; tells the attached powerup

        isHookable = false;
        rigidbody.isKinematic = false;

        // Create a joint between the hook and the hooked object
        joint = gameObject.AddComponent<FixedJoint>();
        Rigidbody hookedRigidbody = hookedTransform.GetComponent<Rigidbody>();
        if(hookedRigidbody) joint.connectedBody = hookedRigidbody;

        // If hit a player, apply damage and knockback
        Hittable hookedHittable = hookedTransform.GetComponent<Hittable>();
        hookedHittable?.HitWithGlobalVelocity(rigidbody.position, currentVelocity, hitDamage, hitKnockbackMultiplier, currentVelocity.magnitude / launchSpeedInitial, GetAttachedPlayer());

        attachedPowerup.OnHookSuccess();
    }

    public void OnHookFailure(){
        // Deactivates collision and tells the attached powerup

        isHookable = false;

        attachedPowerup.OnHookFailure();
    }



    public void Launch(){
        // Gives the hook an initial velocity in the forward direction, and sets off the physics coroutine

        isHookable = true;
        distanceCovered = 0;
        currentVelocity = launchSpeedInitial * transform.forward;
        rigidbody.isKinematic = true;

        StartCoroutine(UpdatePositionLaunching());
    }

    private IEnumerator UpdatePositionLaunching(){
        // Moves the hook through the air until collision flag is raised

        while(isHookable){
            // After traveling in a straight line for a while, apply gravity and drag forces
            if(distanceCovered > straightTravelDistance){
                currentVelocity += gravityAcceleration * Time.fixedDeltaTime * Vector3.down;
                currentVelocity += dragAcceleration * Time.fixedDeltaTime * -transform.forward;
            }

            // Update position
            Vector3 positionChange = currentVelocity * Time.fixedDeltaTime;
            rigidbody.MovePosition(rigidbody.position + positionChange);
            distanceCovered += positionChange.magnitude;

            // Update rotation to always face velocity direction
            rigidbody.MoveRotation(Quaternion.LookRotation(currentVelocity, Vector3.up));

            yield return new WaitForFixedUpdate();
        }
    }

}
