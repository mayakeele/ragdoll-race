using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockoutTrigger : MonoBehaviour
{
    public bool knockoutOnEnter = true;
    public bool knockoutOnExit = false;

    public Collider collider;

    private void Awake()
    {
        if(!collider){
            collider = GetComponent<Collider>();
        }
    }


    
    private void OnTriggerEnter(Collider other)
    {
        if(knockoutOnEnter){
            Hittable hittable = other.gameObject.GetComponent<Hittable>();

            if(hittable){
                Player player = hittable.player;

                player.Kill();
            }
        }   
    }

    private void OnCollisionEnter(Collision other)
    {
        if(knockoutOnEnter){
            Hittable hittable = other.gameObject.GetComponent<Hittable>();

            if(hittable){
                Player player = hittable.player;

                player.Kill();
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if(knockoutOnExit){
            Hittable hittable = other.gameObject.GetComponent<Hittable>();

            if(hittable){
                Player player = hittable.player;

                player.Kill();
            }
        }
        
    }
}
