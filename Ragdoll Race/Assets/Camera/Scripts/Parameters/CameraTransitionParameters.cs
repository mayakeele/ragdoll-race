using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTransitionParameters : MonoBehaviour
{
    [Header("Angle Transition")]
    public AnimationCurve angleTransitionCurve;
    public float angleTransitionTime;


    [Header("FOV Transition")]
    public AnimationCurve fovTransitionCurve;
    public float fovTransitionTime;


    [Header("Padding Transition")]
    public AnimationCurve paddingTransitionCurve;
    public float paddingTransitionTime;


    [Header("Distance Constraints Transition")]
    public AnimationCurve constraintsTransitionCurve;
    public float constraintsTransitionTime;
}
