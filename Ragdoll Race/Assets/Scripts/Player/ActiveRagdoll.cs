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
    [SerializeField] private float targetTorsoHeight;
    [SerializeField] private float targetHeadHeight;

    
    [Header("Force Settings")]
    [SerializeField] private float legsSpringConstant;
    [SerializeField] private float legsSpringDamping;
    [Space]
    [SerializeField] private float torsoSpringConstant;
    [SerializeField] private float torsoSpringDamping;
    [Space]
    [SerializeField] private float headSpringConstant;
    [SerializeField] private float headSpringDamping;


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
        
    }

    void FixedUpdate()
    {
        Vector3 pelvisForce = CalculateUpwardForce(pelvisRigidbody.worldCenterOfMass.y, targetLegsLength, pelvisRigidbody.velocity.y, 8f, legsSpringConstant, legsSpringDamping);
        //Vector3 torsoForce = CalculateUpwardForce(torsoRigidbody.worldCenterOfMass.y, targetTorsoHeight, torsoRigidbody.velocity.y, 7f, torsoSpringConstant, torsoSpringDamping);
        //Vector3 headForce = CalculateUpwardForce(headRigidbody.worldCenterOfMass.y, targetHeadHeight, headRigidbody.velocity.y, 7f, headSpringConstant, headSpringDamping);
        
        pelvisRigidbody.AddForce(pelvisForce);
        //torsoRigidbody.AddForce(torsoForce);
        //headRigidbody.AddForce(headForce);
    }



    // Public Functions




    // Private Functions

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

}
