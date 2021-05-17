using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Rigidbody rigidbody;
    [SerializeField] private bool autoTrackVelocity = true;
    [SerializeField] private bool autoTrackAngularVelocity = true;
    public Vector3 velocity;
    public Vector3 angularVelocity;

    private Vector3 previousPosition;
    private Quaternion previousRotation;

    void Start()
    {
        if(!rigidbody){ rigidbody = GetComponent<Rigidbody>(); }
        
        previousPosition = rigidbody.position;
        previousRotation = rigidbody.rotation;

        velocity = Vector3.zero;
        angularVelocity = Vector3.zero;
    }

    void FixedUpdate()
    {
        if(autoTrackVelocity){
            Vector3 deltaPosition = rigidbody.position - previousPosition;
            velocity = deltaPosition / Time.fixedDeltaTime;

            previousPosition = rigidbody.position;
        }

        if(autoTrackAngularVelocity){
            Vector3 axis = QuaternionExtensions.AxisBetween(previousRotation, rigidbody.rotation, out float angle);
            angularVelocity = angle * Mathf.Deg2Rad * axis / Time.fixedDeltaTime;

            previousRotation = rigidbody.rotation;
        }
    }



    public Vector3 GetPointVelocity(Vector3 worldPosition){
        // Returns the calculated velocity of a given point, assuming it is connected to the moving platform by translation and rotation
        // Takes into account both velocity and angular velocity

        Vector3 translationalVelocity = velocity;

        Vector3 rotationRadiusWorld = worldPosition - rigidbody.position;
        Vector3 rotationRadiusFlat = Vector3.ProjectOnPlane(rotationRadiusWorld, angularVelocity);

        Vector3 rotationalVelocity = Vector3.Cross(rotationRadiusFlat, angularVelocity);

        return rotationalVelocity + translationalVelocity;
    }
}
