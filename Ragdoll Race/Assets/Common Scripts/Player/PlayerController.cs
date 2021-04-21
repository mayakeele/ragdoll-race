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
    [SerializeField] private float airSpeed;
    [SerializeField] private float dizzySpeed;
    [Space]
    [SerializeField] private float walkAcceleration;
    [SerializeField] private float runAcceleration;
    [SerializeField] private float boostAcceleration;
    [SerializeField] private float airAcceleration;
    [SerializeField] private float dizzyAcceleration;
    

    [Header("Jump Settings")]
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float jumpSpringDisableTime;
    [SerializeField] private int jumpPhysicsFrames;


    [Header("Turning Settings")]
    [SerializeField] private float turnSpringConstant;
    [SerializeField] private float turnDampingConstant;


    [Header("Ragdoll Getup Settings")]
    [SerializeField] private float ragdollGetupStickThreshold;



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
        // Update current speed and acceleration limits 
        UpdateMovementSpeed();


        // Get camera space
        Vector3 cameraForward = player.manager.cameraController.GetCameraForwardDirection();
        Vector3 cameraRight = Vector3.Cross(Vector3.up, cameraForward);

        // Calculate the ideal velocity both relative to the ground and in world space
        Vector3 idealRelativeVelocity = ((cameraForward * moveInput.y) + (cameraRight * moveInput.x)) * currMoveSpeedLimit;
        Vector3 idealVelocity = idealRelativeVelocity;
        if(player.activeRagdoll.groundRigidbody){
            idealVelocity += player.activeRagdoll.groundRigidbody.GetPointVelocity(player.activeRagdoll.groundPosition).ProjectHorizontal();
        }

        Vector3 currentVelocity = player.rootRigidbody.velocity.ProjectHorizontal();

        Vector3 requiredVelocityChange = (idealVelocity - currentVelocity);
        float perFrameSpeedChange = currMoveAcceleration * Time.fixedDeltaTime;


        // Disables player movement if they are in the ragdoll state
        if(!player.isRagdoll){

            // Calculate force required to move towards or exactly equal the target velocity without overshoot
            Vector3 movementForce = Vector3.zero;
            if(requiredVelocityChange.magnitude > perFrameSpeedChange){
                movementForce = player.activeRagdoll.GetBodyMass() * currMoveAcceleration * requiredVelocityChange.normalized;
            }
            else{
                movementForce = player.activeRagdoll.GetBodyMass() * requiredVelocityChange / Time.fixedDeltaTime;
            }

            // Apply movement force to player
            player.rootRigidbody.AddForce(player.activeRagdoll.GetBodyMass() * currMoveAcceleration * requiredVelocityChange.normalized);

            // If the player is standing on an object with a rigidbody, apply equal and opposite force
            if(player.activeRagdoll.groundRigidbody){
                player.activeRagdoll.groundRigidbody.AddForceAtPosition(-movementForce, player.activeRagdoll.groundPosition);
            }
        }


        // Turn the ragdoll towards the current movement direction by applying torque
        if(!player.isRagdoll){     
            Vector3 currLookDirection = player.rootForward.transform.forward.ProjectHorizontal();
            Vector3 idealLookDirection = idealRelativeVelocity.ProjectHorizontal();
            Vector3 turningTorque = DampedSpring.GetDampedSpringTorque(currLookDirection, idealLookDirection, player.rootRigidbody.angularVelocity, turnSpringConstant, turnDampingConstant);

            player.rootRigidbody.AddTorque(turningTorque);
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

        // If the stick input exceeds a threshold, try to make player stand upright if they are ragdolled
        if(player.isRagdoll && moveInput.magnitude > ragdollGetupStickThreshold){
            player.TrySetRagdollState(false);
        }
    }

    public void OnJump(InputAction.CallbackContext context){

        // Try to make player stand upright if they are ragdolled
        player.TrySetRagdollState(false);

        //jumpInput = context.action.triggered;
        jumpInput = context.started;

        if(jumpInput && player.isGrounded && !player.isRagdoll){
            StartCoroutine(player.activeRagdoll.PerformJump(jumpSpeed, jumpPhysicsFrames, jumpSpringDisableTime));
        }

    }

    public void OnToggleRagdoll(InputAction.CallbackContext context){
        // Tell the player script that the user wants to toggle ragdoll mode
        if(context.started){
            player.TrySetRagdollState(!player.isRagdoll);
        }
    }



    // Private Functions

    private void UpdateMovementSpeed(){
        // Updates the current speed and acceleration limit
        
        if(player.isDizzy){
            // Dizzy state overrides other states
            currMoveSpeedLimit = dizzySpeed;
            currMoveAcceleration = dizzyAcceleration;
        }
        else if(player.isGrounded){
            // Grounded movement
            currMoveSpeedLimit = runSpeed;
            currMoveAcceleration = runAcceleration;
        }
        else{
            // Air movement
            currMoveSpeedLimit = airSpeed;
            currMoveAcceleration = airAcceleration;
        }
    }
}
