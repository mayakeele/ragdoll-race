using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [Header("Component References")]
    public PlayersManager playersManager;
    public Camera mainCamera;
    public Rigidbody rb;
    public Transform anchorTransform;
    

    [Header("Framing Parameters")]
    public float horizontalAngle;
    public float verticalAngle;
    [Space]
    public float horizontalPaddingDistance;
    public float verticalPaddingDistance;
    [Space]
    public float cameraFOV;



    [Header("Distance Constraints")]
    public float targetBoundsHorizontal;
    public float targetBoundsVertical;


    [Header("Spring Parameters")]
    public float springFrequency;
    public float springDamping;



    [Header("Control Flags")]
    public bool freezeInPlace = false;
    public bool ignoreDistanceConstraints = false;

    //[SerializeField] private LayerMask cameraObstructionLayers;
    //[SerializeField] private LayerMask cameraCollisionLayers;
    //[SerializeField] private float cameraPhysicsRadius;



    // Camera Variables



    // Main Functions

    void Start()
    {
        cameraFOV = mainCamera.fieldOfView;
    }


    void FixedUpdate(){

        List<Player> allPlayers = playersManager.GetAllPlayers();
        int numPlayers = allPlayers.Count;

        if(numPlayers > 0){
            // Get the world space positions of all players' feet and heads, to get full enclosing volume
            List<Vector3> playerFeetPositions = playersManager.GetPositions(allPlayers).AddVector(0, -playersManager.characterPelvisHeight, 0);
            List<Vector3> playerHeadPositions = playerFeetPositions.AddVector(0, playersManager.characterHeadHeight, 0);


            List<Vector3> playerTargetsWorld = new List<Vector3>();
            playerTargetsWorld.AddRange(playerFeetPositions);
            playerTargetsWorld.AddRange(playerHeadPositions);

            List<Vector3> playerTargetsLocal = transform.InverseTransformPoints(playerTargetsWorld);


            // Find the corners of a box surrounding the players in camera space, add padding space
            Vector3 maxDimensionsLocal = playerTargetsLocal.MaxComponents() + new Vector3(horizontalPaddingDistance, verticalPaddingDistance, 0);
            Vector3 minDimensionsLocal = playerTargetsLocal.MinComponents() - new Vector3(horizontalPaddingDistance, verticalPaddingDistance, 0);


            // Clamp the box enclosing the players to fit within the bounds
            ConstrainBoxDimensionsWithinBounds(ref maxDimensionsLocal, ref minDimensionsLocal);


            // Find the local and world space center point of the target box
            Vector3 centerPointLocal = (maxDimensionsLocal + minDimensionsLocal) / 2;
            Vector3 centerPointWorld = transform.TransformPoint(centerPointLocal);
            

            // Calculate the enclosing volume and frame the players within the camera's view
            Vector3 enclosingDimensions = (maxDimensionsLocal - minDimensionsLocal);
            float cameraFramingDistance = CalculateFramingDistance(mainCamera, enclosingDimensions);
            Vector3 targetCameraPosition = (-transform.forward * cameraFramingDistance) + centerPointWorld;


            // Calculate and apply spring forces on the camera
            Vector3 relativePosition = rb.position - targetCameraPosition;
            Vector3 relativeVelocity = rb.velocity - playersManager.AverageVelocity(allPlayers);
            
            Vector3 springAcceleration = DampedSpring.GetDampedSpringAcceleration(relativePosition, relativeVelocity, springFrequency, springDamping);
            rb.AddForce(springAcceleration, ForceMode.Acceleration);
        }
   
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


    private float CalculateFramingDistance(Camera camera, Vector3 boundingDimensions){
        // Returns the ideal distance to place the camera FROM THE CENTROID to frame all players

        Vector2 frameDimensions = new Vector2(boundingDimensions.x, boundingDimensions.y);
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


    private void ConstrainCameraWithinBounds(){
        // Clamps the camera's position relative to the current anchor
        Vector3 anchorPlaneNormal = -transform.forward;
        
        Vector3 cameraPositionRelative = transform.position - anchorTransform.position;

        // Clamp the sideways direction
    }

    private void ConstrainBoxDimensionsWithinBounds(ref Vector3 maxDimensionsLocal, ref Vector3 minDimensionsLocal){
        // Clamps the min and max dimensions of the box surrounding the player targets.
        // Box min and max dimension points are relative to the camera

        Vector3 anchorPositionLocal = transform.InverseTransformPoint(anchorTransform.position);

        Vector3 maxDimensionsRelativeToAnchor = maxDimensionsLocal - anchorPositionLocal;
        Vector3 minDimensionsRelativeToAnchor = minDimensionsLocal - anchorPositionLocal;


        // Clamp horizontal direction
        Vector3 maxHorizontalClamped = Vector3.Project(maxDimensionsRelativeToAnchor, Vector3.right).ClampMagnitude(0, targetBoundsHorizontal);
        Vector3 minHorizontalClamped = Vector3.Project(minDimensionsRelativeToAnchor, Vector3.right).ClampMagnitude(0, targetBoundsHorizontal);

        // Clamp target bounds in the vertical direction
        Vector3 maxVerticalClamped = Vector3.Project(maxDimensionsRelativeToAnchor, Vector3.up).ClampMagnitude(0, targetBoundsVertical);
        Vector3 minVerticalClamped = Vector3.Project(minDimensionsRelativeToAnchor, Vector3.up).ClampMagnitude(0, targetBoundsVertical);


        // Update target box dimensions to match the clamped values
        maxDimensionsLocal = anchorPositionLocal + maxHorizontalClamped + maxVerticalClamped;
        minDimensionsLocal = anchorPositionLocal + minHorizontalClamped + minVerticalClamped;

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



    public void SetParameters(CameraParametersContainer newParameters){
        anchorTransform = newParameters.anchorTransform;

        SetLookAngles(newParameters.horizontalAngle, newParameters.verticalAngle);
        UpdateCameraDirection();

        mainCamera.fieldOfView = newParameters.cameraFOV;

        horizontalPaddingDistance = newParameters.horizontalPaddingDistance;
        verticalPaddingDistance = newParameters.verticalPaddingDistance;

        targetBoundsHorizontal = newParameters.maxDistanceHorizontal;
        targetBoundsVertical = newParameters.maxDistanceVertical;

        springFrequency = newParameters.springFrequency;
        springDamping = newParameters.springDamping;
    }

    public void SetParameters(CameraParametersContainer newParameters, CameraTransitionParameters transitionParameters){

        anchorTransform = newParameters.anchorTransform;

        StartCoroutine(TransitionRotation(
            newParameters.horizontalAngle, newParameters.verticalAngle, 
            transitionParameters.angleTransitionCurve, transitionParameters.angleTransitionTime));


        StartCoroutine(TransitionFOV(newParameters.cameraFOV, transitionParameters.fovTransitionCurve, transitionParameters.fovTransitionTime));


        StartCoroutine(TransitionPadding(newParameters.horizontalPaddingDistance, newParameters.verticalPaddingDistance, 
            transitionParameters.paddingTransitionCurve, transitionParameters.paddingTransitionTime));


        targetBoundsHorizontal = newParameters.maxDistanceHorizontal;
        targetBoundsVertical = newParameters.maxDistanceVertical;


        springFrequency = newParameters.springFrequency;
        springDamping = newParameters.springDamping;
    }



    public void SetLookAngles(float horizontal, float vertical){
        horizontalAngle = horizontal;
        verticalAngle = vertical;

        UpdateCameraDirection();
    }
    public void UpdateCameraDirection(){
        rb.MoveRotation(Quaternion.Euler(verticalAngle, horizontalAngle, 0));
        transform.rotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0);
    }

    public void UpdateFOV(float FOV){
        cameraFOV = FOV;
        mainCamera.fieldOfView = cameraFOV;
    }



    private IEnumerator TransitionRotation(float finalHorizontal, float finalVertical, AnimationCurve transitionCurve, float transitionTime){
        // Transitions the camera's horizontal and vertical rotation from its current values to given values over a given time period, 
        // following a curve from 0 to 1 (start to end)

        float initialHorizontal = horizontalAngle;
        float initialVertical = verticalAngle;

        float currTime = 0;
        while(currTime < transitionTime){
            currTime += Time.fixedDeltaTime;

            float gradient = transitionCurve.Evaluate(currTime / transitionTime);

            horizontalAngle = gradient.Map(0, 1, initialHorizontal, finalHorizontal);
            verticalAngle = gradient.Map(0, 1, initialVertical, finalVertical);

            UpdateCameraDirection();

            yield return new WaitForFixedUpdate();
        }

        SetLookAngles(finalHorizontal, finalVertical);
    }

    private IEnumerator TransitionFOV(float finalFOV, AnimationCurve transitionCurve, float transitionTime){
        // Transitions the camera's FOV from its current value to a given value over a given time period, 
        // following a curve from 0 to 1 (start to end)

        float initialFOV = cameraFOV;

        float currTime = 0;
        while(currTime < transitionTime){
            currTime += Time.deltaTime;

            float gradient = transitionCurve.Evaluate(currTime / transitionTime);

            UpdateFOV(gradient.Map(0, 1, initialFOV, finalFOV));

            yield return new WaitForEndOfFrame();
        }

        UpdateFOV(finalFOV);
    }

    private IEnumerator TransitionPadding(float finalHorizontal, float finalVertical, AnimationCurve transitionCurve, float transitionTime){
        // Transitions the camera's horizontal and vertical padding from its current values to given values over a given time period, 
        // following a curve from 0 to 1 (start to end)

        float initialHorizontal = horizontalPaddingDistance;
        float initialVertical = verticalPaddingDistance;

        float currTime = 0;
        while(currTime < transitionTime){
            currTime += Time.fixedDeltaTime;

            float gradient = transitionCurve.Evaluate(currTime / transitionTime);

            horizontalPaddingDistance = gradient.Map(0, 1, initialHorizontal, finalHorizontal);
            verticalPaddingDistance = gradient.Map(0, 1, initialVertical, finalVertical);

            yield return new WaitForFixedUpdate();
        }

        horizontalPaddingDistance = finalHorizontal;
        verticalPaddingDistance = finalVertical;
    }
}
