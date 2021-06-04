using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnchorMover : MonoBehaviour
{
    public Transform targetTransform;
    [Space]
    public bool copyTargetPosition = true;
    public bool copyTargetRotation = false;


    void FixedUpdate()
    {
        if(copyTargetPosition){
            transform.position = targetTransform.position;
        }

        if(copyTargetRotation){
            transform.rotation = targetTransform.rotation;
        }
    }
}
