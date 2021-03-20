using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoundingVolume
{
    
    public static List<Vector3> GetFarthestPointPair(List<Vector3> allPoints){
        // Returns the pair of points from the set which are furthest away from each other
        
        // Initialize variables
        Vector3 pointA = Vector3.zero;
        Vector3 pointB = Vector3.zero;
        float greatestDistance = 0;
        int numPoints = allPoints.Count;

        // Compare every possible pair of points
        for(int a = 0; a < numPoints - 1; a++){
            for(int b = a + 1; b < allPoints.Count; b++){
                Vector3 currA = allPoints[a];
                Vector3 currB = allPoints[b];

                float currentDist = Vector3.Distance(currA, currB);

                if (currentDist > greatestDistance){
                    greatestDistance = currentDist;
                    pointA = currA;
                    pointB = currB;
                }
            }
        }

        List<Vector3> farthestPoints = new List<Vector3>();
        farthestPoints.Add(pointA);
        farthestPoints.Add(pointB);

        return farthestPoints;
    }
}
