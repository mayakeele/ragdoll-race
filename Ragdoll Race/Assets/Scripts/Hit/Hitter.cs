using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitter : MonoBehaviour
{
    [Header("References")]
    private Rigidbody rigidbody;


    [Header("Hit Criteria")]
    public float minHitSpeed;
    public float maxHitSpeed;

    
    [Header("Damage Properties")]
    public float baseDamage;
    public float maxDamage;


    [Header("Launch Properties")]
    public float baseKnockbackMultiplier;
    public float maxKnockbackMultiplier;

    
    private void OnCollisionEnter(Collision other)
    {
        
        Hittable hitObject = other.gameObject.GetComponent<Hittable>();
        float hitSpeed = rigidbody.velocity.magnitude;

        // Only register hit if the hit object has component Hittable and this object is moving fast enough
        if(hitObject && hitSpeed >= minHitSpeed){

            // Scale damage and launch multiplier by the gradient between the speed floor and ceiling
            float scaleGradient = Mathf.Clamp01(hitSpeed.GradientBetween(minHitSpeed, maxHitSpeed));

            float hitDamage = baseDamage + (scaleGradient * (maxDamage - baseDamage));
            float hitKnockbackMultiplier = baseKnockbackMultiplier + (scaleGradient * (maxKnockbackMultiplier - baseKnockbackMultiplier));

            // Tell the hit Hittable that it has been hit. hit hit hit
            hitObject.Hit(other.GetContact(0).point, other.impulse, hitDamage, hitKnockbackMultiplier);
        }
    }
}
