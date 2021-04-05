using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hittable : MonoBehaviour
{
    [Header("Damage & Knockback Properties")]
    [SerializeField] private float bodyPartDamageMultiplier;
    [SerializeField] private float bodyPartKnockbackMultiplier;


    // Private variables
    private Player player;

    void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    void Update()
    {
        
    }


    // Public functions

    public void Hit(Vector3 hitLocation, Vector3 hitImpulse, float hitDamage, float hitKnockbackMultiplier){
        // Tells the attached player that this limb has been hit, and passes on the hit's damage and knockback multiplier
        player.OnBodyPartHit(this, hitLocation, hitImpulse, hitDamage * bodyPartDamageMultiplier, hitKnockbackMultiplier * bodyPartKnockbackMultiplier);
    }

    // Private functions

    
}
