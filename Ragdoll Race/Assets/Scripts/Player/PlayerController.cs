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
        Vector3 currentVelocity = new Vector3(player.rb.velocity.x, 0, player.rb.velocity.z);

        Vector3 requiredVelocityChange = (idealVelocity - currentVelocity);
        float perFrameSpeedChange = currMoveAcceleration * Time.fixedDeltaTime;


        // Apply only enough force to move towards or exactly equal the target velocity without overshoot
        if(requiredVelocityChange.magnitude > perFrameSpeedChange){
            player.rb.AddForce(requiredVelocityChange.normalized * currMoveAcceleration, ForceMode.Acceleration);
        }
        else{
            player.rb.AddForce(requiredVelocityChange, ForceMode.VelocityChange);
        }
            
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


    // Private Functions

    private void Jump(){
        // Add vertical velocity to the player
        player.rb.velocity = new Vector3(player.rb.velocity.x, jumpSpeed, player.rb.velocity.z);
        player.isGrounded = false;
    }
}
