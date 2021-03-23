using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveRagdoll : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private Player player;
    [SerializeField] private Rigidbody pelvisRigidbody;


    [Header("Detection Settings")]
    [SerializeField] private LayerMask walkableLayers;


    [Header("Body Settings")]
    [SerializeField] private float targetLegsLength;

    
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
        
    }

    void FixedUpdate()
    {
        Vector3 legsForce = CalculateLegsForce(pelvisRigidbody.worldCenterOfMass.y, targetLegsLength, 8f, legsSpringConstant, legsSpringDamping);
        pelvisRigidbody.AddForce(legsForce);
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
    private Vector3 CalculateLegsForce(float currentHeight, float targetHeight, float liftedMass, float springConstant, float dampingConstant){
        // Calculates the upwards force of the legs on the hips based on the legs' compression

        Vector3 legsForce;

        //if(currentHeight > targetHeight){
            // Legs are fully extended, no force
            legsForce = Vector3.zero;
        //}
        //else{
            // Legs are partially compressed, acting as a damped spring
            Vector3 relativePosition = new Vector3(0, currentHeight - targetHeight, 0);
            Vector3 relativeVelocity = new Vector3(0, pelvisRigidbody.velocity.y, 0);
            legsForce = DampedSpring.GetDampedSpringForce(relativePosition, relativeVelocity, liftedMass, springConstant, dampingConstant);
        //}

        return legsForce;
    }

}
