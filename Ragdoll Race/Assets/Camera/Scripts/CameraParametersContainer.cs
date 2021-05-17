using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraParametersContainer : MonoBehaviour
{
    [Header("Component References")]
    public Transform anchorTransform;
    

    [Header("Framing Parameters")]
    public float horizontalAngle;
    public float verticalAngle;
    [Space]
    public float horizontalPaddingDistance;
    public float verticalPaddingDistance;
    [Space]
    public float cameraFOV;


    [Header("Distance Constraints")]
    public float maxDistanceForward;
    public float maxDistanceHorizontal;
    public float maxDistanceVertical;


    [Header("Spring Parameters")]
    public float springFrequency;
    public float springDamping;



    public void SetCameraControllerParameters(CameraController cam){
        cam.horizontalAngle = horizontalAngle;
        cam.verticalAngle = verticalAngle;

        cam.horizontalPaddingDistance = horizontalPaddingDistance;
        cam.verticalPaddingDistance = verticalPaddingDistance;

        cam.cameraFOV = cameraFOV;

        cam.maxDistanceForward = maxDistanceForward;
        cam.maxDistanceHorizontal = maxDistanceHorizontal;
        cam.maxDistanceVertical = maxDistanceVertical;

        cam.springFrequency = springFrequency;
        cam.springDamping = springDamping;
    }
}
