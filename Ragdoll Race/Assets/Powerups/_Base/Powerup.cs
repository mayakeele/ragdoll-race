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




    public Texture2D GetIcon(){
        return icon;
    }





    // Main Lifetime Events

    public virtual void OnPickup(){

    }

    public virtual void OnActivate(){

    }

    public virtual void OnRemove(){
        Destroy(this.gameObject);
    }



    // Input Events

    public virtual void OnInputMove(Vector2 movementInput){

    }

    public virtual void OnInputJump(){

    }

    public virtual void OnInputArmsUp(){
        
    }

    public virtual void OnInputArmsDown(){

    }
}
