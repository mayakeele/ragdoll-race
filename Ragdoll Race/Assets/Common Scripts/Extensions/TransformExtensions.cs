using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions
{
    
    public static List<Vector3> GetPositions(this List<Transform> transforms){
        // Gets a list of the positions of all transforms in the given list
        List<Vector3> posList = new List<Vector3>();
        
        foreach (Transform trans in transforms){
            posList.Add(trans.position);
        }

        return posList;
    }

    public static List<Quaternion> GetRotations(this List<Transform> transforms){
        // Gets a list of the positions of all transforms in the given list
        List<Quaternion> rotList = new List<Quaternion>();
        
        foreach (Transform trans in transforms){
            rotList.Add(trans.rotation);
        }
        
        return rotList;
    }
}
