using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private ControllerInputs inputSystem;
    [SerializeField] private Player thisPlayer;
    [SerializeField] private Rigidbody rb;


    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float boostSpeed;
    [SerializeField] private float jumpSpeed;



    // Input Variables
    Vector2 moveInput = Vector2.zero;
    bool jumpInput = false;




    // Main Functions

    void Awake(){

        InitializeInput();
    }

    void Start()
    {

    }


    void FixedUpdate()
    {
        Vector3 moveSpeed = new Vector3(moveInput.x, 0, moveInput.y) * walkSpeed;
        thisPlayer.transform.Translate(moveSpeed * Time.fixedDeltaTime);

        if (jumpInput){
            rb.velocity = new Vector3(rb.velocity.x, jumpSpeed, rb.velocity.z);
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

    private void InitializeInput(){
        // Initializes Input System and connects actions to functions
        inputSystem = new ControllerInputs();
    }

    
}
