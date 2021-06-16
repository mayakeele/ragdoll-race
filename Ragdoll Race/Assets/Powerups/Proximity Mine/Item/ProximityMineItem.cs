using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityMineItem : SpawnedItem
{
    [Header("Activation Properties")]
    [SerializeField] private float initialDisarmDuration;
    [SerializeField] private float activationTime;
    [SerializeField] private LayerMask triggerableLayers;


    [Header("Explosion Properties")]
    [SerializeField] private float explosionRadius;
    [SerializeField] private float explosionDamage;
    [SerializeField] private float explosionForce;


    private bool isArmed = false;
    private bool isExploding = false;



    void Start()
    {
        StartCoroutine(ArmMineAfterWait(initialDisarmDuration));
    }

    
    void OnTriggerEnter(Collider other)
    {
        if(isArmed && !isExploding && triggerableLayers.ContainsLayer(other.gameObject.layer)){
            StartCoroutine(Explode());
        }
    }


    private IEnumerator ArmMineAfterWait(float waitTime){
        yield return new WaitForSeconds(waitTime);
        isArmed = true;

        Debug.Log("mine is armed");
    }


    private IEnumerator Explode(){
        // Gets all rigidbodies within the explosion radius and applies a force to them
        // Also applies damage to any detected players. Then destroys self and powerup attached

        isExploding = true;

        // Immediately play sound and flash a light before explosion

        // Delay a bit, then create explosion VFX and calculate explosion radius
        yield return new WaitForSeconds(0);

        // Destroy self
        Destroy(this.gameObject);
    }
}
