using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockoutTrigger : MonoBehaviour
{
    [Header("Trigger Properties")]
    public bool knockoutOnEnter = true;
    public bool knockoutOnExit = false;


    [Header("Override KO Effect Prefab")]
    [SerializeField] private GameObject knockoutEffectPrefab;


    private Collider collider;



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
                Player thisPlayer = hittable.player;

                if(knockoutEffectPrefab) thisPlayer.TryKnockout(knockoutEffectPrefab);
                else thisPlayer.TryKnockout();
            }
        }   
    }


    private void OnTriggerExit(Collider other)
    {
        if(knockoutOnExit){
            Hittable hittable = other.gameObject.GetComponent<Hittable>();

            if(hittable){
                Player thisPlayer = hittable.player;

                if(knockoutEffectPrefab) thisPlayer.TryKnockout(knockoutEffectPrefab);
                else thisPlayer.TryKnockout(); 
            }
        }
        
    }
}
