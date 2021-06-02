using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    
    public static List<int> FindIndices<T>(this T[] array, T target){
        List<int> indices = new List<int>();

        for(int i = 0; i < array.Length; i++){
            if (array[i].Equals(target)){
                indices.Add(i);
            }
        }
        
        return indices;
    }

    public static List<int> FindIndices<T>(this List<T> list, T target){
        List<int> indices = new List<int>();

        for(int i = 0; i < list.Count; i++){
            if (list[i].Equals(target)){
                indices.Add(i);
            }
        }
        
        return indices;
    }

    public static void SetAllValues<T>(this List<T> list, T value){
        for(int i = 0; i < list.Count; i++){
            list[i] = value;
        }
    }


    public static void ActivateOneDeactivateOthers(this List<GameObject> objectList, int activeIndex){
        // Activates the object at the specified index, and deactivates all other objects in the list

        foreach(GameObject otherObject in objectList){
            otherObject.SetActive(false);
        }

        objectList[activeIndex].SetActive(true);
    }

}
