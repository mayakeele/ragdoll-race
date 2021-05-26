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
    public float maxDistanceHorizontal;
    public float maxDistanceVertical;


    [Header("Spring Parameters")]
    public float springFrequency;
    public float springDamping;

}
