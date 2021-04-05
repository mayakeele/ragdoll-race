using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FloatExtensions
{

    public static float GradientBetween(this float value, float rangeStart, float rangeEnd){
        // Returns the percentage of the way the value is between the start and end values
        return (value - rangeStart) / (rangeEnd - rangeStart);
    }

    public static float Map(this float value, float originalStart, float originalEnd, float newStart, float newEnd){
        // Returns a value that is the same percentage of the way between the new bounds as value is between the original bounds
        float gradient = value.GradientBetween(originalStart, originalEnd);
        float newValue = newStart + (gradient * (newEnd - newStart));
        
        return newValue;
    }

}
