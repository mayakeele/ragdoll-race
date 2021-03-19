using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [Header("Component References")]
    public PlayersManager playersManager;
    
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

    public Transform testCube;



    // Main Functions

    void Start()
    {

    }


    void Update(){

        List<Player> allPlayers = playersManager.GetAllPlayers();

        // Get the world space positions of all players' feet and heads, to get full enclosing volume
        List<Vector3> playerFeetPositions = playersManager.PlayerPositions(allPlayers);
        List<Vector3> playerHeadPositions = playerFeetPositions.AddVector(new Vector3(0, playersManager.characterHeight, 0));

        List<Vector3> playerVolumeWorld = new List<Vector3>();
        playerVolumeWorld.AddRange(playerFeetPositions);
        playerVolumeWorld.AddRange(playerHeadPositions);

        List<Vector3> playerVolumeLocal = mainCamera.transform.InverseTransformPoints(playerVolumeWorld);

        // Find the bounding box surrounding the players
        Vector3 maxBoundsLocal = playerVolumeLocal.MaxComponents();
        Vector3 minBoundsLocal = playerVolumeLocal.MinComponents();

        // Find the local and world space center point of the players
        Vector3 centerPointLocal = (maxBoundsLocal + minBoundsLocal) / 2;
        Vector3 centerPointWorld = mainCamera.transform.TransformPoint(centerPointLocal);

        // Extract position dimensions of the rectangular prism enclosing the players, 
        Vector3 enclosingDimensions = (maxBoundsLocal - minBoundsLocal);

        // Increase the enclosing volume's x component to account for the characters' radii
        //enclosingDimensions += new Vector3(playersManager.characterRadius * 2, 0, 0);

        testCube.position = mainCamera.transform.TransformPoint(centerPointLocal);
        testCube.rotation = Quaternion.LookRotation(mainCamera.transform.forward, Vector3.up);
        testCube.localScale = enclosingDimensions;
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
