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

        Vector3 deltaV = (idealVelocity - currentVelocity);


        // Stop the player's velocity if their speed is below a certain threshold and they aren't trying to change their velocity
        if(player.rb.velocity.magnitude < stopSpeedThreshold  &&  deltaV.magnitude <= stopSpeedThreshold){
            player.rb.velocity = Vector3.zero;
        }
        // Otherwise, apply the force
        else{
            player.rb.AddForce(deltaV.normalized * currMoveAcceleration, ForceMode.Acceleration);
        }
        

        // Jump if the player is grounded and presses jump
        if(jumpInput && player.isGrounded){
            player.rb.velocity = new Vector3(player.rb.velocity.x, jumpSpeed, player.rb.velocity.z);
            player.isGrounded = false;
        }
    }


    void OnEnable(){
        inputSystem.Gameplay.Enable();
    }
    void OnDisable(){
        inputSystem.Gameplay.Disable();
    }



    // Public Functions

    public void OnMove(InputAction.CallbackContext context){
        moveInput = context.action.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context){
        jumpInput = context.action.triggered;
    }


    // Private Functions

}
