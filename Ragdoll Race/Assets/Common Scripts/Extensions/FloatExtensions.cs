using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FloatExtensions
{

    // Scalar float functions

    public static float Gradient(this float value, float rangeStart, float rangeEnd){
        // Returns the percentage of the way the value is between the start and end values
        return (value - rangeStart) / (rangeEnd - rangeStart);
    }

    public static float GradientClamped(this float value, float rangeStart, float rangeEnd){
        // Returns the percentage of the way the value is between the start and end values
        float gradient = (value - rangeStart) / (rangeEnd - rangeStart);
        
        return Mathf.Clamp01(gradient);
    }

    public static float Map(this float value, float originalStart, float originalEnd, float newStart, float newEnd){
        // Returns a value that is the same percentage of the way between the new bounds as value is between the original bounds
        
        float gradient = value.Gradient(originalStart, originalEnd);
        float newValue = newStart + (gradient * (newEnd - newStart));
        
        return newValue;
    }

    public static float MapClamped(this float value, float originalStart, float originalEnd, float newStart, float newEnd){
        // Returns a value that is the same percentage of the way between the new bounds as value is between the original bounds, clamped between min and max ends
        
        float gradient = value.Gradient(originalStart, originalEnd);
        float newValue = newStart + (gradient * (newEnd - newStart));

        newValue = newEnd > newStart ? Mathf.Clamp(newValue, newStart, newEnd) : Mathf.Clamp(newValue, newEnd, newStart);
        
        return newValue;
    }

    public static float MapPercent(this float percent, float newStart, float newEnd){
        // Returns a value that is the same percentage of the way between the new bounds as percent is between 0 and 1
        
        float newValue = newStart + (percent * (newEnd - newStart));
        
        return newValue;
    }

    public static float MapPercentClamped(this float percent, float newStart, float newEnd){
        // Returns a value that is the same percentage of the way between the new bounds as percent is between 0 and 1, clamped between min and max ends
        
        float newValue = newStart + (percent * (newEnd - newStart));

        newValue = newEnd > newStart ? Mathf.Clamp(newValue, newStart, newEnd) : Mathf.Clamp(newValue, newEnd, newStart);
        
        return newValue;
    }



    // Float array functions

    public static int Sum(this int[] array){
        int sum = 0;
        for (int i = 0; i < array.Length; i++){
            sum += array[i];
        }
        return sum;
    }

    public static float Sum(this float[] array){
        float sum = 0;
        for (int i = 0; i < array.Length; i++){
            sum += array[i];
        }
        return sum;
    }

    public static int Max(this int[] array){
        int max = array[0];
        foreach (int num in array){
            if (num > max){
                max = num;
            }
        }
        return max;
    }

    public static int Min(this int[] array){
        int min = array[0];
        foreach (int num in array){
            if (num < min){
                min = num;
            }
        }
        return min;
    }


    public static bool IsRealNumber(this float f){
        // Returns false if the float is NaN, positive infinity, or negative infinity
        if(float.IsNaN(f) || float.IsNegativeInfinity(f) || float.IsPositiveInfinity(f)){
            return false;
        }
        else{
            return true;
        }
    }


    public static float AngleDifference( float angle1, float angle2 ){
        float diff = ( angle2 - angle1 + 180 ) % 360 - 180;
        return diff < -180 ? diff + 360 : diff;
    }


    
}
