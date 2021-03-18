using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extensions
{
    
    public static Vector3 ProjectHorizontal(this Vector3 inVector){
        // Projects the input vector onto the XZ plane
        return Vector3.ProjectOnPlane(inVector, Vector3.up);
    }
}
