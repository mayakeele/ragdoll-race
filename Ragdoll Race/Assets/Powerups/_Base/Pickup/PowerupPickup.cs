using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupPickup : MonoBehaviour
{
    [Header("Properties")]
    public Powerup powerup;
    public bool destroySelfOnPickup = true;

    [Header("Effects")]
    [SerializeField] private Transform effectsLocation;
    [SerializeField] private GameObject spawnEffect;
    [SerializeField] private GameObject destroyEffect;


    private Collider pickupTrigger;
    private bool isActive;



    void Awake(){
        pickupTrigger = GetComponent<Collider>();
        isActive = false;

        if(destroyEffect) Instantiate(spawnEffect, effectsLocation.position, effectsLocation.rotation);
    }


    private void OnTriggerEnter(Collider other)
    {
        Hittable hittable = other.GetComponent<Hittable>();

        // If a player is detected, try to give them a powerup
        if(hittable){
            bool wasPowerupGiven = hittable.player.powerupManager.TrySetPowerup(powerup);

            if(wasPowerupGiven && destroySelfOnPickup){
                RemoveSelf();
            }
        }
        
    }


    private void RemoveSelf(){
        if(destroyEffect) Instantiate(destroyEffect, effectsLocation.position, effectsLocation.rotation);
        Destroy(this.gameObject);
    }
}
