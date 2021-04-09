using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hittable : MonoBehaviour
{
    
    [Header("Damage & Knockback Properties")]
    [SerializeField] private float bodyPartDamageMultiplier;
    [SerializeField] private float bodyPartKnockbackMultiplier;


    [Header("Sound Effects")]
    [SerializeField] private List<AudioClip> hitSoundClips;
    [Range(0,1)] [SerializeField] private float hitVolume;
    [SerializeField] private float hitPitchMin;
    [SerializeField] private float hitPitchMax;



    // Private variables
    private Player player;
    private AudioSource audioSource;



    void Awake()
    {
        player = GetComponentInParent<Player>();
        audioSource = player.audioSource;
    }

    void Update()
    {
        
    }



    // Public functions

    public void Hit(Vector3 hitLocation, Vector3 hitImpulse, float hitDamage, float hitKnockbackMultiplier){
        // Tells the attached player that this limb has been hit, and passes on the hit's damage and knockback multiplier
        player.OnBodyPartHit(this, hitLocation, hitImpulse, hitDamage * bodyPartDamageMultiplier, hitKnockbackMultiplier * bodyPartKnockbackMultiplier);

        // Play impact sound at hit location
        if(hitSoundClips.Count > 0){
            audioSource.PlayClipPitchShifted(RandomExtensions.RandomChoice(hitSoundClips), hitVolume, hitPitchMin, hitPitchMax);
        }
    }



    // Private functions

    
}
