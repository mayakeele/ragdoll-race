using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitter : MonoBehaviour
{
    
    [Header("References")]
    private Rigidbody rigidbody;
    private Player attachedPlayer;


    [Header("Hit Criteria")]
    public float minHitSpeed;
    public float maxHitSpeed;

    
    [Header("Knockback & Damage Properties")]
    public float knockbackMultiplier;
    public float baseDamage;
    public float maxScaledDamage;


    [Header("Sound Effects")]
    [SerializeField] private AudioSource audioSource;
    [Space]
    [SerializeField] private List<AudioClip> playerHitSounds;
    [Range(0,1)] [SerializeField] private float playerHitVolume;
    [SerializeField] private float playerHitPitchMin;
    [SerializeField] private float playerHitPitchMax;
    [Space]
    [SerializeField] private List<AudioClip> groundHitSounds;
    [Range(0,1)] [SerializeField] private float groundHitVolume;
    [SerializeField] private float groundHitPitchMin;
    [SerializeField] private float groundHitPitchMax;


    [Header("Particle Prefabs")]
    [SerializeField] private GameObject playerHitParticlePrefab;
    [SerializeField] private GameObject groundHitParticlePrefab;



    // Private Variables

    private Vector3 preCollisionVelocity = Vector3.zero;


    // Unity Functions

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
  
    }


    private void Start()
    {
        // If this hitter also has a hittable attached (meaning it is a player's body part) take note of the parent player
        Hittable hittable = GetComponent<Hittable>();
        if(hittable){
            attachedPlayer = hittable.player;
        }
    }


    private void FixedUpdate()
    {
        // Keep track of the velocity before collision occurs
        preCollisionVelocity = rigidbody.velocity;
    }
    

    private void OnCollisionEnter(Collision other)
    {
        
        Hittable hitObject = other.gameObject.GetComponent<Hittable>();
        Vector3 relativeVelocity = -other.relativeVelocity;

        // If other object has component Hittable, it is a player
        // Make sure the player isn't hitting themself
        if(hitObject && hitObject.player != attachedPlayer){

            // Register hit if the RELATIVE speed is fast enough and THIS speed is fast enough (hitter is active, not passive)
            if(preCollisionVelocity.magnitude >= minHitSpeed && relativeVelocity.magnitude >= minHitSpeed){

                // Scale damage by the gradient between the speed floor and ceiling
                float hitDamage = relativeVelocity.magnitude.MapClamped(minHitSpeed, maxHitSpeed, baseDamage, maxScaledDamage);

                Vector3 hitLocation = other.GetContact(0).point;
                float hitSpeedGradient = relativeVelocity.magnitude.GradientClamped(minHitSpeed, maxHitSpeed);

                // Tell the hit Hittable that it has been hit, receive whether the hit was successful
                bool hitSuccessful = hitObject.Hit(hitLocation, relativeVelocity, hitDamage, knockbackMultiplier, hitSpeedGradient);
                if(hitSuccessful){
                    // Play impact sound at hit location
                    if(playerHitSounds.Count > 0){
                        audioSource.PlayClipPitchShifted(RandomExtensions.RandomChoice(playerHitSounds), playerHitVolume, playerHitPitchMin, playerHitPitchMax);
                    }

                    // Create hit particle effects


                    // If this hitter object is part of the player, give them brief immunity to avoid blowback
                    //if(attachedPlayer){
                        //attachedPlayer.TriggerImmunity();
                    //}
                }
                
            }
            
        }

        // Otherwise it is solid geometry
        else{

            // Scale SFX volume and particle intensity by the hit speed

            // Play sound effect
            if(groundHitSounds.Count > 0){
                //audioSource.PlayClipPitchShifted(RandomExtensions.RandomChoice(groundHitSounds), groundHitVolume, groundHitPitchMin, groundHitPitchMax);
            }

            // Create particle effect
        }
    }
}
