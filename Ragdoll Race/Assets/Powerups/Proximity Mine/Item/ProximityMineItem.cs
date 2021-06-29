using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityMineItem : ExplosiveItem
{
    [Header("Activation Properties")]
    [SerializeField] private float initialDisarmDuration;
    [SerializeField] private float triggerDuration;
    [SerializeField] private float autoExplodeAfterTime;
    [SerializeField] private LayerMask triggerableLayers;


    [Header("Effects")]
    [SerializeField] private GameObject warningEffect;



    private SphereCollider triggerCollider;
    private bool isArmed = false;
    private bool isExploding = false;



    void Start()
    {
        triggerCollider = GetComponent<SphereCollider>();

        StartCoroutine(ArmMineAfterWait(initialDisarmDuration));
    }

    
    void OnTriggerEnter(Collider other)
    {
        if(isArmed && !isExploding && triggerableLayers.ContainsLayer(other.gameObject.layer)){
            StartCoroutine(ExplodeMine());
        }
    }



    private IEnumerator ArmMineAfterWait(float waitTime){
        yield return new WaitForSeconds(waitTime);
        isArmed = true;
        StartCoroutine(QueueAutomaticExplosion());
    }


    private IEnumerator QueueAutomaticExplosion(){
        // Sets self up to automatically explode after a set period of time if nobody enters the trigger
        yield return new WaitForSeconds(autoExplodeAfterTime);

        if(isArmed && !isExploding){
            StartCoroutine(ExplodeMine());
        }
    }



    private IEnumerator ExplodeMine(){
        // Gets all rigidbodies within the explosion radius and applies a force to them
        // Also applies damage to any detected players. Then destroys self and powerup attached

        isExploding = true;

        // Immediately spawn warning effects before explosion
        if(warningEffect) Instantiate(warningEffect, transform.position, transform.rotation);

        // Delay a bit, then explode
        yield return new WaitForSeconds(triggerDuration);
        Explode(transform.position, true);
    }
}
