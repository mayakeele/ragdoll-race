using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Powerup : MonoBehaviour
{
    public enum PowerupCategory {        
        PassiveEffect,
        EntitySpawner,
        ActiveWeapon,
        HandAttachment,
        FeetAttachment,
        TorsoAttachment,
        BodyTransformation,
        KinematicMover
    }

    public enum LifetimeCountdownStart {
        Never,
        OnPickup,
        OnActivate
    }

    private static readonly bool[,] powerupCategoryCompatibility = {
        {true,true,true,true,true,true,true,true},
        {false,false,false,false,false,false,false,false},
        {false,false,false,false,false,false,false,false},
        {true,true,false,false,true,true,false,true},
        {true,true,true,true,false,true,false,true},
        {true,true,true,true,true,false,false,true},
        {false,false,false,false,false,false,false,false},
        {false,false,false,false,false,false,false,false}
    };



    public PowerupCategory category;
    public bool canBePassive;
    [Space]
    public float powerupLifetime;
    public LifetimeCountdownStart lifetimeCountdownStart;
    
    [Space]
    [SerializeField] private Texture2D icon;



    [HideInInspector] public PlayerPowerupManager attachedPowerupManager;
    [HideInInspector] public Player attachedPlayer;

    [HideInInspector] public bool hasActivated = false;



    public Texture2D GetIcon(){
        return icon;
    }

    public static bool ArePowerupsCompatible(PowerupCategory incomingCategory, PowerupCategory currentCategory){
        return powerupCategoryCompatibility[(int)currentCategory, (int)incomingCategory];
    }



    // Main Lifetime Events

    public virtual void OnPickup(){

    }


    public virtual bool OnActivateInitial(){
        // Returns whether the initial activation was successful
        return true;
    }
    
    public virtual void OnActivateContinued(){

    }

    public virtual void OnActivateButtonReleased(){

    }


    public virtual void OnRemove(){

    }



    // Input Events

    public void OnInputActivate(){

        if(!hasActivated){
            hasActivated = OnActivateInitial();
        }
        else{
            OnActivateContinued();
        }

        
    }

    public virtual void OnInputMove(Vector2 movementInput){

    }

    public virtual void OnInputJump(){

    }

    public virtual void OnInputArmsUp(){
        
    }

    public virtual void OnInputArmsDown(){

    }
}
