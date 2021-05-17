using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirplaneTargetFollower : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private Transform targetTransform;


    [Header("Spring Properties")]
    [SerializeField] private float springFrequency;
    [SerializeField] private float springDampingRatio;
    [Space]
    [SerializeField] private float angularSpringConstant;
    [SerializeField] private float angularSpringDampingConstant;



    // Unity Functions
    void Start()
    {
        
    }


    void FixedUpdate()
    {
        // Linear spring
        Vector3 springForce = DampedSpring.GetDampedSpringAcceleration(transform.position, targetTransform.position, rigidbody.velocity, springFrequency, springDampingRatio);
        rigidbody.AddForce(springForce, ForceMode.Acceleration);

        // Angular spring
        //Vector3 pitchRollTorque = DampedSpring.GetDampedSpringTorque(transform.up, targetTransform.up, rigidbody.angularVelocity, angularSpringConstant, angularSpringDampingConstant);
        //rigidbody.AddTorque(pitchRollTorque, ForceMode.Force);
        Vector3 torque = DampedSpring.GetDampedSpringTorque(this.transform, targetTransform, rigidbody.angularVelocity, angularSpringConstant, angularSpringDampingConstant);
        rigidbody.AddTorque(torque, ForceMode.Force);
    }
}
