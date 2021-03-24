using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    [Header("Component References")]
    [SerializeField] private ControllerInputs inputSystem;
    [SerializeField] private Player player;


    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float boostSpeed;
    [Space]
    [SerializeField] private float walkAcceleration;
    [SerializeField] private float runAcceleration;
    [SerializeField] private float boostAcceleration;
    [Space]
    [SerializeField] private float stopSpeedThreshold;
    [Space]
    [SerializeField] private float jumpSpeed;


    [Header("Turning Settings")]
    [SerializeField] private float turnSpringConstant;
    [SerializeField] private float turnDampingConstant;



    // Input Variables
    Vector2 moveInput = Vector2.zero;
    bool jumpInput = false;


    // Movement Variables
    float currMoveSpeedLimit;
    float currMoveAcceleration;



    // Main Functions

    void Awake(){
        inputSystem = new ControllerInputs();
    }

    void Start()
    {
        currMoveSpeedLimit = runSpeed;
        currMoveAcceleration = runAcceleration;
    }


    void FixedUpdate()
    {

        // Get camera space axes
        Vector3 cameraForward = player.manager.cameraController.GetCameraForwardDirection();
        Vector3 cameraRight = Vector3.Cross(Vector3.up, cameraForward);

        // Calculate and apply movement force to player
        Vector3 idealVelocity = ((cameraForward * moveInput.y) + (cameraRight * moveInput.x)) * currMoveSpeedLimit;
        Vector3 currentVelocity = player.rootRigidbody.velocity.ProjectHorizontal();

        Vector3 requiredVelocityChange = (idealVelocity - currentVelocity);
        float perFrameSpeedChange = currMoveAcceleration * Time.fixedDeltaTime;


        // Apply only enough force to move towards or exactly equal the target velocity without overshoot
        if(requiredVelocityChange.magnitude > perFrameSpeedChange){
            player.rootRigidbody.AddForce(player.activeRagdoll.GetBodyMass() * currMoveAcceleration * requiredVelocityChange.normalized);
        }
        else{
            //player.rootRigidbody.AddForce(requiredVelocityChange, ForceMode.VelocityChange);
            player.rootRigidbody.AddForce(player.activeRagdoll.GetBodyMass() * requiredVelocityChange / Time.fixedDeltaTime);
        }


        // Turn the ragdoll towards the current movement direction by applying torque
        Vector3 currLookDirection = player.rootRigidbody.transform.forward.ProjectHorizontal();
        Vector3 idealLookDirection = idealVelocity.ProjectHorizontal();
        Vector3 turningTorque = DampedSpring.GetDampedSpringTorque(currLookDirection, idealLookDirection, player.rootRigidbody.angularVelocity, turnSpringConstant, turnDampingConstant);

        player.rootRigidbody.AddTorque(turningTorque);
    }


    void OnEnable(){
        inputSystem.Gameplay.Enable();
    }
    void OnDisable(){
        inputSystem.Gameplay.Disable();
    }



    // Input Events

    public void OnMove(InputAction.CallbackContext context){
        moveInput = context.action.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context){
        //jumpInput = context.action.triggered;
        jumpInput = context.started;

        if(jumpInput && player.isGrounded){
            Jump();
        }
    }

    public void OnToggleRagdoll(InputAction.CallbackContext context){
        // Begin ragdoll state if the button has just been pressed, end ragdoll state if the button is released
        if(context.started){
            //player.SetRagdollState(true);
            player.SetRagdollState(!player.isRagdoll);
            
        }
        //else if(context.canceled){
            //player.SetRagdollState(false);
            //Debug.Log("upright");
        //}
    }



    // Private Functions

    private void Jump(){
        // Add vertical velocity to the player
        player.rootRigidbody.velocity = new Vector3(player.rootRigidbody.velocity.x, jumpSpeed, player.rootRigidbody.velocity.z);
        //player.isGrounded = false;
    }
}
