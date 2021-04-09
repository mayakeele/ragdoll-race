using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PhysicsExtensions
{
    public static bool IsInsideCollider(this Vector3 point, Collider c)
     {
         Vector3 closest = c.ClosestPoint(point);
         if (closest == point){
             return true;
         }
         else{
             return false;
         }
     }
}
