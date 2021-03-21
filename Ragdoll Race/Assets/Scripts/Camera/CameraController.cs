using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [Header("Component References")]
    public PlayersManager playersManager;
    public Camera mainCamera;
    public Rigidbody rb;
    

    [Header("Camera Control Properties")]
    //[SerializeField] private float cameraSpeedH;
    //[SerializeField] private float cameraSpeedV;
    //[SerializeField] private float cameraOrbitRadius;
    //[SerializeField] private float minCameraAngleV;
    //[SerializeField] private float maxCameraAngleV;
    //[SerializeField] private bool invertCameraH;
    //[SerializeField] private bool invertCameraV;


    [Header("Smart Camera Settings")]
    [SerializeField] private float springFrequency;
    [SerializeField] private float springDamping;
    //[SerializeField] private LayerMask cameraObstructionLayers;
    //[SerializeField] private LayerMask cameraCollisionLayers;
    //[SerializeField] private float cameraPhysicsRadius;
    //[SerializeField] private float maxCameraFOV;
    //[SerializeField] private float minCameraFOV;
    [SerializeField] private float horizontalPaddingDistance;
    [SerializeField] private float verticalPaddingDistance;


    // Input Variables
    float camInputH;
    float camInputV;


    // Camera Variables
    float camAngleH = 0;
    float camAngleV = 0;
    float currFOV;



    // Main Functions

    void Start()
    {
        currFOV = mainCamera.fieldOfView;
    }


    void FixedUpdate(){

        List<Player> allPlayers = playersManager.GetAllPlayers();


        // Get the world space positions of all players' feet and heads, to get full enclosing volume
        List<Vector3> playerFeetPositions = playersManager.GetPositions(allPlayers);
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


        // Calculate the enclosing volumeand increase x to account for the characters' radii
        Vector3 enclosingDimensions = (maxBoundsLocal - minBoundsLocal);
        enclosingDimensions += new Vector3(playersManager.characterRadius * 2, 0, 0);


        // Frame the players within the camera's view
        float cameraFramingDistance = CalculateFramingDistance(mainCamera, enclosingDimensions, horizontalPaddingDistance, verticalPaddingDistance);
        Vector3 targetCameraPosition = (-mainCamera.transform.forward * cameraFramingDistance) + centerPointWorld;


        // Calculate spring forces on the camera
        Vector3 relativePosition = transform.position - targetCameraPosition;
        Vector3 relativeVelocity = rb.velocity - playersManager.AverageVelocity(allPlayers);
        
        Vector3 springAcceleration = DampedSpring.GetDampedSpringAcceleration(relativePosition, relativeVelocity, springFrequency, springDamping);
        rb.AddForce(springAcceleration, ForceMode.Acceleration);
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

    private Vector2 AltitudeAzimuthBetween(Vector3 startPos, Vector3 endPos, Vector3 perspectivePos){
        // Returns the horizontal and vertical (azimuth and altitude) angle between two vectors when seen from a perspective postion

        Vector3 startDir = (startPos - perspectivePos).normalized;
        Vector3 endDir = (endPos = perspectivePos).normalized;

        Vector3 middlePlaneNormal = startPos - endPos;

        float hAngle = Vector3.Angle(startDir.ProjectHorizontal(), endDir.ProjectHorizontal());    
        float vAngle = Vector3.Angle(Vector3.ProjectOnPlane(startDir, middlePlaneNormal), Vector3.ProjectOnPlane(endDir, middlePlaneNormal));
        
        return new Vector2(hAngle, vAngle);
    }


    private float CalculateFramingDistance(Camera camera, Vector3 boundingDimensions, float hPadding, float vPadding){
        // Returns the ideal distance to place the camera FROM THE CENTROID to frame all players

        Vector2 frameDimensions = new Vector2(boundingDimensions.x + hPadding, boundingDimensions.y + vPadding);
        float dist;

        // Determine whether the constraining dimension is horizontal or vertical
        if (frameDimensions.x/frameDimensions.y >= camera.aspect){
            // Constrain by width
            float hFOV = Camera.VerticalToHorizontalFieldOfView(camera.fieldOfView, camera.aspect);
            dist = frameDimensions.x / (2 * Mathf.Tan(Mathf.Deg2Rad * hFOV / 2));
        }
        else{
            // Constrain by height
            float vFOV = camera.fieldOfView;
            dist = frameDimensions.y / (2 * Mathf.Tan(Mathf.Deg2Rad * vFOV / 2));
        }

        // Add the distance from the center of the bounding box to the frame (the front face)
        dist += boundingDimensions.z / 2;

        return dist;
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
