using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityMineItem : SpawnedItem
{
    [Header("Activation Properties")]
    [SerializeField] private float initialDisarmDuration;
    [SerializeField] private float triggerDuration;
    [SerializeField] private float autoExplodeAfterTime;
    [SerializeField] private LayerMask triggerableLayers;


    [Header("Explosion Properties")]
    [SerializeField] private float explosionRadius;
    [SerializeField] private float explosionKnockbackMultiplier;
    [Space]
    [SerializeField] private float explosionDamageCenter;
    [SerializeField] private float explosionDamageEdge;
    [Space]
    [SerializeField] private float explosionSpeedCenter;
    [SerializeField] private float explosionSpeedEdge;


    [Header("Effects")]
    [SerializeField] private GameObject warningEffect;
    [SerializeField] private GameObject explosionEffect;



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
            StartCoroutine(Explode());
        }
    }



    private IEnumerator ArmMineAfterWait(float waitTime){
        yield return new WaitForSeconds(waitTime);
        isArmed = true;
        StartCoroutine(QueueAutomaticExplosion());
    }


    private IEnumerator QueueAutomaticExplosion(){
        // Sets self up to automatically explode after a set period of time if nobody explodes it
        yield return new WaitForSeconds(autoExplodeAfterTime);
        StartCoroutine(Explode());
    }



    private IEnumerator Explode(){
        // Gets all rigidbodies within the explosion radius and applies a force to them
        // Also applies damage to any detected players. Then destroys self and powerup attached

        isExploding = true;

        // Immediately spawn warning effects before explosion
        if(warningEffect) Instantiate(warningEffect, transform.position, transform.rotation);

        // Delay a bit, then create explosion VFX
        yield return new WaitForSeconds(triggerDuration);
        if(explosionEffect) Instantiate(explosionEffect, transform.position, transform.rotation);


        // Get a list of all colliders in the explosion radius
        List<Collider> collidersInRadius = new List<Collider>(Physics.OverlapSphere(transform.position, explosionRadius, triggerableLayers));

        // For each player body part in the explosion radius, create a hit by scaling damage and knockback speed by distance
        foreach(Collider collider in collidersInRadius){

            Hittable hittable = collider.gameObject.GetComponent<Hittable>();

            if(hittable){
                Vector3 centerToBodyPart = hittable.transform.position - transform.position;

                Vector3 direction = centerToBodyPart.normalized;
                float centerDist = centerToBodyPart.magnitude;
                float distGradient = centerDist / explosionRadius;

                float scaledDamage = distGradient.MapPercentClamped(explosionDamageCenter, explosionDamageEdge);
                float scaledSpeed = distGradient.MapPercentClamped(explosionSpeedCenter, explosionSpeedEdge);

                Hittable lowerTorsoTarget = hittable.player.activeRagdoll.torsoLowerTransform.GetComponent<Hittable>();
                lowerTorsoTarget.Hit(hittable.transform.position, scaledSpeed * direction, scaledDamage, explosionKnockbackMultiplier, distGradient, GetAttachedPlayer(), true);
            }
        }

        // Destroy self
        Destroy(this.gameObject);
    }
}
