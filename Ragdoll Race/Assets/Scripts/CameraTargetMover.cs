using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTargetMover : MonoBehaviour
{
    [Header("Component References")]
    public List<Player> players;
    public Transform defaultCameraOrbitOrigin;
    private Rigidbody rb;


    [Header("Spring Properties")]
    public float maxOffset;
    public float naturalFrequency;
    public float dampingRatio;


    [Header("Target Variables")]
    public Vector3 referenceFrameVelocity = Vector3.zero;



    void Awake(){
        rb = GetComponent<Rigidbody>();
    }


    void Start()
    {
        transform.position = defaultCameraOrbitOrigin.position;
    }


    void FixedUpdate()
    {
        // Calculate the displacement and velocity of the target, relative to the player
        Vector3 displacement = GetDisplacement();
        Vector3 relativeVelocity = rb.velocity - referenceFrameVelocity;
        

        // Calculate the spring acceleration and apply it to the target
        Vector3 springAcceleration = DampedSpring.GetDampedSpringAcceleration(displacement, relativeVelocity, naturalFrequency, dampingRatio);
        rb.AddForce(springAcceleration, ForceMode.Acceleration);
    }



    public Vector3 GetDisplacement(){
        // Returns the vector of this target's displacement from its origin
        return transform.position - defaultCameraOrbitOrigin.position;
    }

    public float GetDisplacementMagnitude(){
        // Returns the magnitude of this target's displacement from its origin
        return (transform.position - defaultCameraOrbitOrigin.position).magnitude;
    }
}
