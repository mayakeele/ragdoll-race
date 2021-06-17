using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Powerup : MonoBehaviour
{
    //[Header("Events")]
    //public UnityEvent OnPickup;
    //public UnityEvent OnActivate;
    //public UnityEvent OnDeactivate;


    [SerializeField] private Texture2D icon;

    [HideInInspector] public PlayerPowerupManager attachedPowerupManager;


    private bool hasActivated = false;




    public Texture2D GetIcon(){
        return icon;
    }



    void Awake()
    {
        OnPickup();
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

    public virtual void OnInputActivate(){
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
