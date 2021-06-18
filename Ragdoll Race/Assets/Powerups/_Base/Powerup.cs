using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Powerup : MonoBehaviour
{
    public enum PowerupCategory{
        PassiveEffect,
        ItemSpawner,
        ActiveWeapon,
        PassiveHandAttachment,
        PassiveFeetAttachment,
        PassiveTorsoAttachment,
        Transformation
    }


    private static readonly bool[,] powerupCategoryCompatibility = {
        {true,true,true,true,true,true,true},
        {false,false,false,false,false,false,false},
        {false,false,false,false,false,false,false},
        {true,true,false,false,true,true,false},
        {true,true,true,true,false,true,false},
        {true,true,true,true,true,false,false},
        {false,false,false,false,false,false,false}
    };



    public PowerupCategory category;
    public bool canBePassive;
    [Space]
    [SerializeField] private Texture2D icon;



    [HideInInspector] public PlayerPowerupManager attachedPowerupManager;
    private bool hasActivated = false;



    public Texture2D GetIcon(){
        return icon;
    }

    public static bool ArePowerupsCompatible(PowerupCategory incomingCategory, PowerupCategory currentCategory){
        return powerupCategoryCompatibility[(int)currentCategory, (int)incomingCategory];
    }



    // Main Lifetime Events

    public virtual void OnPickup(){

    }

    public virtual void OnActivateInitial(){

    }
    
    public virtual void OnActivateContinued(){

    }

    public virtual void OnRemove(){

    }



    // Input Events

    public void OnInputActivate(){
        if(!hasActivated){
            OnActivateInitial();
        }
        else{
            OnActivateContinued();
        }

        hasActivated = true;
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
