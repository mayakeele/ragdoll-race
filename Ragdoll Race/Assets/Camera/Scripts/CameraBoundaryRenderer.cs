using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class CameraBoundaryRenderer : MonoBehaviour
{
    public CameraParametersContainer parametersContainer;
    public CameraController cameraController;

    public bool setCameraParameters; //"run" or "generate" for example
 
    private void OnValidate()
    {
        if (setCameraParameters) SetCameraParameters ();
    
        setCameraParameters = false;
    }
 
    void SetCameraParameters ()
    {
        //parametersContainer = GetComponent<CameraParametersContainer>();
        //cameraController = FindObjectOfType<CameraController>();

        cameraController.SetParameters(parametersContainer);
        Debug.Log("Set new camera parameters");
    }


    private void OnDrawGizmosSelected()
    {
        
        if(parametersContainer){

            Transform anchor = parametersContainer.anchorTransform;

            Matrix4x4 rotationMatrix = Matrix4x4.TRS(anchor.position, anchor.rotation, anchor.lossyScale);
            Gizmos.matrix = rotationMatrix; 

            float width = parametersContainer.maxDistanceHorizontal * 2;
            float height = parametersContainer.maxDistanceVertical * 2;
            float depth = 0.1f;

            Gizmos.color = Color.red;

            Gizmos.DrawWireCube(Vector3.zero, new Vector3(width, height, depth));
        }
    }

}
