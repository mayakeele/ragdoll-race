using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveItem : SpawnedItem
{
    [Header("Explosion Properties")]
    [SerializeField] private float explosionRadius;
    [SerializeField] private float explosionKnockbackMultiplier;
    [Space]
    [SerializeField] private float explosionDamageCenter;
    [SerializeField] private float explosionDamageEdge;
    [Space]
    [SerializeField] private float explosionSpeedCenter;
    [SerializeField] private float explosionSpeedEdge;
    [Space]
    [SerializeField] private LayerMask affectedLayers;
    [Space]
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private float cameraShakeTrauma;



    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }



    public void Explode(Vector3 explosionCenterPosition, bool destroySelf, List<Player> unaffectedPlayers = null){
        // Deals damage and knockback to any players caught in the blast radius

        if(unaffectedPlayers == null) unaffectedPlayers = new List<Player>();

        // Instantiate the explosion effects if it exists
        if(explosionEffect) Instantiate(explosionEffect, transform.position, transform.rotation);

        // Apply camera shake
        GetAttachedPlayer().manager.cameraController.AddCameraShake(cameraShakeTrauma);

        // Get a list of all colliders in the explosion radius
        List<Collider> collidersInRadius = new List<Collider>(Physics.OverlapSphere(explosionCenterPosition, explosionRadius, affectedLayers));

        // For each player body part in the explosion radius, create a hit by scaling damage and knockback speed by distance
        foreach(Collider collider in collidersInRadius){

            Hittable hittable = collider.gameObject.GetComponent<Hittable>();

            if(hittable && !unaffectedPlayers.Contains(hittable.player)){
                Vector3 centerToBodyPart = hittable.transform.position - explosionCenterPosition;

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
        if(destroySelf) Destroy(this.gameObject);
    }

    public void Explode(Vector3 explosionCenterPosition, bool destroySelf, Player unaffectedPlayer){
        List<Player> players = new List<Player>{unaffectedPlayer};
        Explode(explosionCenterPosition, destroySelf, players);
    }
}
