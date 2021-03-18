using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [Header("Component References")]
    public List<Player> allPlayers;
    public Camera mainCamera;
    


    [Header("Camera Control Properties")]
    [SerializeField] private float cameraSpeedH;
    [SerializeField] private float cameraSpeedV;
    [SerializeField] private float cameraOrbitRadius;
    [SerializeField] private float minCameraAngleV;
    [SerializeField] private float maxCameraAngleV;
    [SerializeField] private bool invertCameraH;
    [SerializeField] private bool invertCameraV;


    [Header("Smart Camera Settings")]
    [SerializeField] private LayerMask cameraObstructionLayers;
    [SerializeField] private LayerMask cameraCollisionLayers;
    [SerializeField] private float cameraPhysicsRadius;
    [SerializeField] private float maxCameraFOV;
    [SerializeField] private float minCameraFOV;
    [SerializeField] private float playerEdgePaddingRatio;


    // Input Variables
    float camInputH;
    float camInputV;



    // Camera Variables
    float camAngleH = 0;
    float camAngleV = 0;
    float timeSinceManualCamMove;



    // Main Functions

    void Start()
    {

    }


    void Update(){

    }


    void LateUpdate()
    {
        
    }



    // Public Functions

    public Vector3 GetCameraForwardDirection(){
        // Calculates the direction the camera is facing, projected onto the horizontal plane
        Vector3 camForwardDir = mainCamera.transform.forward.ProjectHorizontal().normalized;
        return camForwardDir;
    }



    // Private Functions

    private Vector3 CalculateAveragePlayerPosition(){
        return Vector3.zero;
    }


    


    private bool IsLineOfSightClear(Vector3 targetPos, Vector3 cameraPos, float castDistance, float sphereRadius, LayerMask obstructingLayers){
        // Casts a sphere from the target to the camera to determine if there are any obstructions directly in the way
        Ray LOSRay = new Ray(targetPos, cameraPos - targetPos);

        if(Physics.SphereCast(LOSRay, sphereRadius, out RaycastHit hitInfo, castDistance, obstructingLayers)){
            return false;
        }
        else{
            return true;
        }
    }


    private Vector3 GetUnobstructedCameraPosition(Vector3 targetPos, Vector3 cameraPos, float castDistance, float sphereRadius, LayerMask obstructingLayers){
        // Performs a spherecast from the player to the ideal camera position to determine where to place the camera without intersecting geometry
        Ray LOSRay = new Ray(targetPos, cameraPos - targetPos);

        if(Physics.SphereCast(LOSRay, sphereRadius, out RaycastHit hitInfo, castDistance, obstructingLayers)){
            Vector3 sphereCenter = hitInfo.point + (hitInfo.normal * sphereRadius);
            return sphereCenter;
        }
        else{
            return cameraPos;
        }
    }

    
    private Vector3 ValidateCameraPosition(Vector3 targetPos, Vector3 cameraPos, float sphereRadius, LayerMask invalidLayers){
        // Check if the camera's bubble is intersecting any geometry it shouldn't collide with. If it is, calculate a valid position

        if(Physics.OverlapSphere(cameraPos, sphereRadius, invalidLayers).Length > 0){
            // Camera is currently intersecting geometry; perform spherecast from player to camera to get furthest valid position
            float castDistance = (cameraPos - targetPos).magnitude;
            Vector3 validCameraPos = GetUnobstructedCameraPosition(targetPos, cameraPos, castDistance, sphereRadius, invalidLayers);

            return validCameraPos;
        }
        else{
            // Camera position is valid
            return cameraPos;
        }
    }


    private void UpdateCursorLock(){
        // Lock cursor if player clicks on game window, unlock if escape is pressed
        if (Cursor.lockState == CursorLockMode.None && Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (Cursor.lockState == CursorLockMode.Locked && Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
