using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPowerupManager : MonoBehaviour
{
    [HideInInspector] public Player player;

    [SerializeField] private Powerup activePowerup;
    [SerializeField] private Powerup passivePowerup;


    private bool isPowerupHeld;
    private bool isPowerupActive;

    

    void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    void Start()
    {

    }



    // Methods

    public Powerup GetActivePowerup(){
        return activePowerup;
    }
    public Powerup GetPassivePowerup(){
        return passivePowerup;
    }

    public bool ActiveSlotTaken(){
        return activePowerup != null;
    }
    public bool PassiveSlotTaken(){
        return passivePowerup != null;
    }



    public bool TrySetPowerup(Powerup incomingPowerup){
        // Attempts to set the incoming powerup to the active slot, if the slot is empty and the
        // Powerup in the passive slot is compatible.
        // Creates a new instance of the given Powerup and attaches it to this manager

        if(!ActiveSlotTaken()){
            // Make sure incoming and passive are compatible, if passive even exists
            if(!PassiveSlotTaken() || Powerup.ArePowerupsCompatible(incomingPowerup.category, passivePowerup.category)){
                SetActivePowerup(incomingPowerup);
                return true;
            }
            // Incoming and passive are not compatible 
            else{
                return false;
            }
        }
        // Active slot is full
        else{
            return false;
        }
    }

    private void SetActivePowerup(Powerup incomingPowerup){
        Powerup powerupCopy = Object.Instantiate(incomingPowerup);
        activePowerup = powerupCopy;
        activePowerup.attachedPowerupManager = this;
        activePowerup.OnPickup();
    }



    public void RemovePowerup(Powerup powerupToRemove){
        // Removes the given powerup if it exists. Also attempts to shift the active powerup to passive slot,
        // If the passive slot is open and the powerup is able to be passive

        // Destroy all objects and references to the powerup
        if(activePowerup == powerupToRemove) activePowerup = null;
        if(passivePowerup == powerupToRemove) passivePowerup = null;

        powerupToRemove.OnRemove();
        Destroy(powerupToRemove.gameObject);
        powerupToRemove = null;

        // Shift if applicable
        TryMoveActiveToPassive();
    }

    public void RemoveAllPowerups(){
        // Removes both active and passive powerups if they exist
        if(ActiveSlotTaken()){
            RemovePowerup(activePowerup);
        }
        if(PassiveSlotTaken()){
            RemovePowerup(passivePowerup);
        }
    }

    private bool TryMoveActiveToPassive(){
        // Attempts to move the currently active powerup to the passive slot
        if(ActiveSlotTaken() && !PassiveSlotTaken() && activePowerup.canBePassive){
            passivePowerup = activePowerup;
            activePowerup = null;
            return true;
        }
        else{
            return false;
        }
    }



    // Input passers-on

    public void OnActivate(InputAction.CallbackContext context){
        if(context.started){
            if(ActiveSlotTaken()) activePowerup.OnInputActivate();
            if(PassiveSlotTaken()) passivePowerup.OnInputActivate();

            TryMoveActiveToPassive();
        } 
    }

    public void OnMove(InputAction.CallbackContext context){
        Vector2 moveInput = context.action.ReadValue<Vector2>();

        if(ActiveSlotTaken()) activePowerup.OnInputMove(moveInput);
        if(PassiveSlotTaken()) passivePowerup.OnInputMove(moveInput);

    }

    public void OnJump(InputAction.CallbackContext context){
        if(ActiveSlotTaken() && context.started) activePowerup.OnInputJump();
        if(PassiveSlotTaken() && context.started) passivePowerup.OnInputJump();
    }

    public void OnArmAction(InputAction.CallbackContext context){
        if(context.started){
            if(ActiveSlotTaken()) activePowerup.OnInputArmsUp();
            if(PassiveSlotTaken()) passivePowerup.OnInputArmsUp();
        }
        else if(context.canceled){
            if(ActiveSlotTaken()) activePowerup.OnInputArmsDown();
            if(PassiveSlotTaken()) passivePowerup.OnInputArmsDown();
        }
    }

}
