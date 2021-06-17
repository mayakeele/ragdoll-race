using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPowerupManager : MonoBehaviour
{
    [HideInInspector] public Player player;

    private Powerup currentPowerup;

    [SerializeField] private Powerup initialPowerup;

    private bool isPowerupHeld;
    private bool isPowerupActive;

    
    void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    void Start()
    {
        SetPowerup(initialPowerup, true);
    }


    public Powerup GetCurrentPowerup(){
        return currentPowerup;
    }

    public bool SetPowerup(Powerup powerup, bool overrideCurrent = false){
        // Creates a new instance of the powerup given and attaches it to this manager

        Powerup powerupCopy = Object.Instantiate(powerup);

        if(overrideCurrent || HasPowerup()){
            currentPowerup = powerupCopy;
            currentPowerup.attachedPowerupManager = this;

            return true;
        }
        else{
            return false;
        }
    }

    public void RemovePowerup(){
        if(HasPowerup()){
            currentPowerup.OnRemove();
            Destroy(currentPowerup.gameObject);
            currentPowerup = null;
        }
    }


    public bool HasPowerup(){
        return currentPowerup != null;
    }


    // Input passers-on

    public void OnActivate(InputAction.CallbackContext context){
        if(HasPowerup() && context.started) currentPowerup.OnInputActivate();
    }

    public void OnMove(InputAction.CallbackContext context){
        Vector2 moveInput = context.action.ReadValue<Vector2>();
        if(HasPowerup()) currentPowerup.OnInputMove(moveInput);
    }

    public void OnJump(InputAction.CallbackContext context){
        if(HasPowerup() && context.started) currentPowerup.OnInputJump();
    }

    public void OnArmAction(InputAction.CallbackContext context){
        if(context.started){
            if(HasPowerup()) currentPowerup.OnInputArmsUp();
        }
        else if(context.canceled){
            if(HasPowerup()) currentPowerup.OnInputArmsDown();
        }
    }

}
