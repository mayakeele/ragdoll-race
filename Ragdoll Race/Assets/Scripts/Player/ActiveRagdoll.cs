using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveRagdoll : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private Player player;
    [SerializeField] private Rigidbody pelvisRigidbody;
    [SerializeField] private Rigidbody torsoRigidbody;
    [SerializeField] private Rigidbody headRigidbody;
    [SerializeField] private List<Rigidbody> bodyPartRigidbodies;


    [Header("Detection Settings")]
    [SerializeField] private LayerMask walkableLayers;


    [Header("Body Settings")]
    [SerializeField] private float targetLegsLength;
    private float bodyMass;

    
    [Header("Force Settings")]
    [SerializeField] private float legsSpringConstant;
    [SerializeField] private float legsSpringDamping;


    //[SerializeField] private float torsoFollowForce;
    //[SerializeField] private float headFollowForce;
    //[SerializeField] private float armsFollowForce;
    //[SerializeField] private float legsFollowForce;



    // Unity Functions
    void Awake()
    {
        
    }

    void Start()
    {
        RecalculateBodyMass();
    }

    void FixedUpdate()
    {   
        // Cast a ray downwards to determine the floor height
        if(Physics.Raycast(pelvisRigidbody.worldCenterOfMass, Vector3.down, out RaycastHit hitInfo, targetLegsLength, walkableLayers)){

            // Apply an upward spring force on the pelvis if it is near the floor
            float targetHeight = hitInfo.point.y + targetLegsLength;
            Vector3 pelvisForce = CalculateUpwardForce(pelvisRigidbody.worldCenterOfMass.y, targetHeight, pelvisRigidbody.velocity.y, bodyMass, legsSpringConstant, legsSpringDamping);    
            
            pelvisRigidbody.AddForce(pelvisForce);

            //player.isGrounded = true;
        }
        //else{
        //    player.isGrounded = false;
        //}
    }



    // Public Functions
    
    public float GetBodyMass(){
        return bodyMass;
    }

    public void SetPelvisRotationConstraint(bool constrainY){
        // Constrains pelvis rotation to either the Y axis only, or no rotation at all
        if(constrainY){
            pelvisRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
        }
        else{
            pelvisRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
    }

    public void RemoveRotationConstraints(){
        // Removes all constraints on the pelvis rigidbody's rotation
        pelvisRigidbody.constraints = RigidbodyConstraints.None;
    }


    // Private Functions

    private void RecalculateBodyMass(){
        bodyMass = 0;
        foreach(Rigidbody rb in bodyPartRigidbodies){
            bodyMass += rb.mass;
        }
    }

    /*private Vector3 CalculateLegsForce(float currentHeight, float targetHeight, float springFrequency, float springDamping){
        // Calculates the upwards force of the legs on the hips based on the legs' compression

        Vector3 legsForce;

        if(currentHeight > targetHeight){
            // Legs are fully extended, no force
            legsForce = Vector3.zero;
        }
        else{
            // Legs are partially compressed, acting as a damped spring
            Vector3 relativePosition = new Vector3(0, currentHeight - targetHeight, 0);
            Vector3 relativeVelocity = new Vector3(0, pelvisRigidbody.velocity.y, 0);
            legsForce = DampedSpring.GetDampedSpringForce(relativePosition, relativeVelocity, springFrequency, springDamping);
        }

        return legsForce;
    }*/

    private Vector3 BodyCenterOfMass(){
        // Calculates the world space center of mass of the ragdoll's body parts
        float totalMass = 0;
        Vector3 centerOfMass = Vector3.zero;

        foreach(Rigidbody rb in bodyPartRigidbodies){
            totalMass += rb.mass;

            centerOfMass += rb.mass * rb.worldCenterOfMass;
        }

        return centerOfMass / totalMass;
    }


    private Vector3 CalculateUpwardForce(float currentHeight, float targetHeight, float verticalSpeed, float liftedMass, float springConstant, float dampingConstant){
        // Calculates the upwards force of a virtual spring, like a marionette doll

        Vector3 upwardForce;

        if(currentHeight > targetHeight){
            // Legs are fully extended, no force
            upwardForce = Vector3.zero;
        }
        else{
            // Legs are partially compressed, acting as a damped spring
            Vector3 relativePosition = new Vector3(0, currentHeight - targetHeight, 0);
            Vector3 relativeVelocity = new Vector3(0, verticalSpeed, 0);
            upwardForce = DampedSpring.GetDampedSpringForce(relativePosition, relativeVelocity, liftedMass, springConstant, dampingConstant);
        }

        return upwardForce;
    }
    private Vector3 CalculateUpwardForce(float displacementHeight, float verticalSpeed, float liftedMass, float springConstant, float dampingConstant){
        // Calculates the upwards force of a virtual spring, like a marionette doll

        Vector3 upwardForce;

        if(displacementHeight > 0){
            // Legs are fully extended, no force
            upwardForce = Vector3.zero;
        }
        else{
            // Legs are partially compressed, acting as a damped spring
            Vector3 relativePosition = new Vector3(0, displacementHeight, 0);
            Vector3 relativeVelocity = new Vector3(0, verticalSpeed, 0);
            upwardForce = DampedSpring.GetDampedSpringForce(relativePosition, relativeVelocity, liftedMass, springConstant, dampingConstant);
        }

        return upwardForce;
    }

}
