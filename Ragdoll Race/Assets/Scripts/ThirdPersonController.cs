using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{

    [Header("Component References")]
    public Camera mainCamera;
    public Transform cameraOrbitOrigin;
    public Transform cameraFocusTarget;


    [Header("Movement Translation Properties")]
    public float walkSpeed;
    public float walkAcceleration;
    public float sprintSpeed;
    public float sprintAcceleration;
    public float jumpSpeed;


    [Header("Movement Rotation Properties")]
    public float turnSpeed;
    public float maxSmoothTurnAngle;
    public float maxSnapAngle;


    [Header("Camera Control Properties")]
    public float cameraSpeedH;
    public float cameraSpeedV;
    public float cameraOrbitRadius;
    public float minCameraAngleV;
    public float maxCameraAngleV;
    public bool invertCameraH;
    public bool invertCameraV;


    [Header("Smart Camera Settings")]
    public LayerMask cameraObstructionLayers;
    public LayerMask cameraCollisionLayers;
    public float cameraPhysicsRadius;
    public float maxCameraFOV;
    public float minCameraFOV;


    // Input Variables
    float movementInputH;
    float movementInputV;
    Vector3 movementInputClamped;
    bool sprintInput;
    bool jumpInput;
    float camInputH;
    float camInputV;


    // Movement Variables
    float currMovementSpeedLimit;
    float currMovementAccelerationLimit;
    float currForwardSpeed;
    bool isSprinting;


    // Camera Variables
    float camAngleH = 0;
    float camAngleV = 0;
    float timeSinceManualCamMove;



    void Start()
    {
        currMovementSpeedLimit = walkSpeed;
        isSprinting = false;
        //currMovementSpeedLimit = sprintSpeed;
        //isSprinting = true;

        camAngleH = transform.eulerAngles.y;  
    }


    void Update(){
        // Gather all player movement inputs
        movementInputH = Input.GetAxisRaw("Horizontal");
        movementInputV = Input.GetAxisRaw("Vertical");
        movementInputClamped = Vector3.ClampMagnitude(new Vector3(movementInputH, 0, movementInputV), 1);

        jumpInput = Input.GetButtonDown("Jump");

        sprintInput = Input.GetButton("Sprint");

        UpdateCursorLock();


        // Allow player to move if they are on the ground
        if(playerManager.isGrounded){
            
            // Update the sprinting state if the sprint button is held
            if(sprintInput == true){
                isSprinting = true;
                currMovementSpeedLimit = sprintSpeed;
            }
            else{
                isSprinting = false;
                currMovementSpeedLimit = walkSpeed;
            }


            // Calculate the angle between the desired direction and the current direction
            Vector3 playerForward = transform.forward.ProjectHorizontal();
            Vector3 desiredMovement = GetDesiredMovementVector();

            float turnAngle = Vector3.SignedAngle(playerForward, desiredMovement, Vector3.up);
            float turnAngleAbsolute = Mathf.Abs(turnAngle);
            float turnSign = Mathf.Sign(turnAngle);

            
            
            // If the player is sprinting and tries to turn sharply, perform a skid
            if(turnAngleAbsolute > maxSmoothTurnAngle && isSprinting){
                // Trigger skid animation in new direction
                // Rapidly decelerate
                transform.LookAt(transform.position + desiredMovement, Vector3.up);
            }

            // If the turn angle is small enough or the player is moving slowly, perform a smooth turn while maintaining speed
            else{

                // If the turning angle is able to be covered in a single frame, immediately snap to the desired angle
                if(turnAngleAbsolute <= turnSpeed * Time.deltaTime){
                    transform.LookAt(transform.position + desiredMovement, Vector3.up);
                }

                // Otherwise, continue to turn the player towards the movement direction
                else{
                    Quaternion turnQuaternion = Quaternion.AngleAxis(turnSpeed * turnSign * Time.deltaTime, Vector3.up);
                    Vector3 newLookDirection = turnQuaternion * playerForward;
                    transform.LookAt(transform.position + newLookDirection, Vector3.up);
                }
            }
            

            // Apply pseudo-force to the player rigidbody to move
            transform.Translate(desiredMovement * currMovementSpeedLimit * Time.deltaTime, Space.World);
            
        }
    }


    void LateUpdate()
    {
        // Gather all player camera inputs
        camInputH = Input.GetAxis("Mouse X");
        camInputV = Input.GetAxis("Mouse Y");


        // Update camera angle variables if the player is locked into the tab
        if(Cursor.lockState == CursorLockMode.Locked){
            camAngleH += camInputH * cameraSpeedH * Time.deltaTime * (invertCameraH ? -1 : 1);
            camAngleH = camAngleH % 360;

            camAngleV += camInputV * cameraSpeedV * Time.deltaTime * (invertCameraV ? -1 : 1);
            camAngleV = Mathf.Clamp(camAngleV, minCameraAngleV, maxCameraAngleV);
        }
        

        // Calculate the ideal camera position and rotation
        Vector3 camOffsetDir = Quaternion.Euler(-camAngleV, camAngleH, 0) * -Vector3.forward;
        Vector3 idealCamPosition = cameraOrbitOrigin.position + (camOffsetDir * cameraOrbitRadius);


        // Perform spherecast to surroundings to determine whether the environment is obstructing the character
        Vector3 newCamPosition = GetUnobstructedCameraPosition(cameraOrbitOrigin.position, idealCamPosition, cameraOrbitRadius, cameraPhysicsRadius, cameraObstructionLayers);

        // Make sure the camera is not intersecting any geometry it shouldn't
        newCamPosition = ValidateCameraPosition(cameraOrbitOrigin.position, newCamPosition, cameraPhysicsRadius, cameraCollisionLayers);


        // Set the camera's positon and look target
        mainCamera.transform.position = newCamPosition;
        mainCamera.transform.LookAt(cameraOrbitOrigin);
    }



    private Vector3 GetDesiredMovementVector(){
        // Uses player input and the current camera view to determine which way the player wants to go

        Vector3 camForwardDir = mainCamera.transform.forward.ProjectHorizontal().normalized;
        Vector3 camRightDir = Vector3.Cross(Vector3.up, camForwardDir).normalized;
        Vector3 movementVector = ((camRightDir * movementInputClamped.x) + (camForwardDir * movementInputClamped.z));

        return movementVector;
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
