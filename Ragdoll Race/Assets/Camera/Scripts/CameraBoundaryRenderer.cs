using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class CameraBoundaryRenderer : MonoBehaviour
{
    public CameraParametersContainer parametersContainer;
    public CameraController cameraController;

    public bool setCameraParameters;
    public bool showGizmo;
 
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


    private void OnDrawGizmos()
    {
        
        if(showGizmo && parametersContainer){

            Transform anchor = parametersContainer.anchorTransform;

            Matrix4x4 rotationMatrix = Matrix4x4.TRS(anchor.position, cameraController.transform.rotation, Vector3.one);
            Gizmos.matrix = rotationMatrix; 

            float width = parametersContainer.maxDistanceHorizontal * 2;
            float height = parametersContainer.maxDistanceVertical * 2;
            float depth = 0.1f;

            Gizmos.color = Color.red;

            Gizmos.DrawWireCube(Vector3.zero, new Vector3(width, height, depth));
        }
    }

}
