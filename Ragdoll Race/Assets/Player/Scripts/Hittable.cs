using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hittable : MonoBehaviour
{
    
    [Header("Damage & Knockback Properties")]
    [SerializeField] private float bodyPartDamageMultiplier;
    [SerializeField] private float bodyPartKnockbackMultiplier;


    [Header("Hitstun (Forced Ragdoll) Properties")]
    [SerializeField] private bool canForceRagdollOnHit;
    [SerializeField] private float forceRagdollDurationMin;
    [SerializeField] private float forceRagdollDurationMax;


    [Header("Camera Shake Effects")]
    [SerializeField] private float minCameraShakeOnHit;
    [SerializeField] private float maxCameraShakeOnHit;


    [Header("Sound Effects")]
    [SerializeField] private List<AudioClip> hitSoundClips;
    [Range(0,1)] [SerializeField] private float hitVolume = 1;
    [SerializeField] private float hitPitchMin = 1;
    [SerializeField] private float hitPitchMax = 1;



    // Private variables
    [HideInInspector] public Player player;
    [HideInInspector] public Rigidbody rigidbody;
    private AudioSource audioSource;
    private CameraShakeManager cameraShakeManager;



    void Awake()
    {
        player = GetComponentInParent<Player>();
        audioSource = player.audioSource;

        rigidbody = GetComponent<Rigidbody>();

        cameraShakeManager = FindObjectOfType<CameraShakeManager>();
    }

    void Update()
    {
        
    }



    // Public functions

    public bool Hit(Vector3 hitLocation, Vector3 hitRelativeVelocity, float hitDamage, float hitKnockbackMultiplier, float hitSpeedGradient, Player attacker = null){
        // Tells the attached player that this limb has been hit, and passes on the hit's damage and knockback multiplier
        // Returns whether the hit was successful (if the player is not immune)

        float ragdollDuration = canForceRagdollOnHit ? hitSpeedGradient.MapClamped(0,1, forceRagdollDurationMin, forceRagdollDurationMax) : 0;

        bool hitSuccessful = player.OnBodyPartHit(this, hitLocation, hitRelativeVelocity, hitDamage * bodyPartDamageMultiplier, hitKnockbackMultiplier * bodyPartKnockbackMultiplier, ragdollDuration, attacker);

        
        if(hitSuccessful){
            
            // Play impact sound at hit location 
            if(hitSoundClips.Count > 0){
                audioSource.PlayClipPitchShifted(RandomExtensions.RandomChoice(hitSoundClips), hitVolume, hitPitchMin, hitPitchMax);
            }
            
            // Shake the camera
            float cameraShakeAmount = hitSpeedGradient.MapPercentClamped(minCameraShakeOnHit, maxCameraShakeOnHit);
            cameraShakeManager.AddCameraShake(cameraShakeAmount);

            return true;
        }

        else{
            return false;
        }
        
    }


    // Private functions

    
}
