using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Component References")]
    [HideInInspector] public PlayersManager manager;
    public ActiveRagdoll activeRagdoll;
    public Rigidbody rootRigidbody;
    public Transform rootForward;
    public AudioSource audioSource;
    [SerializeField] private string managerTag = "PlayersManager";


    [Header("Damage & Knockback Properties")]
    [SerializeField] private float knockbackMultiplierAt0;
    [SerializeField] private float knockbackMultiplierAt100;
    [Space]
    [SerializeField] private float maxDamage;
    [SerializeField] private float hitImmunityDuration;


    [Header("State Variables")]
    public float currentDamage = 0;
    public bool isGrounded;
    public bool isRagdoll;
    public bool isDizzy;
    public bool isImmune;
    private float immunityTimer;



    // Unity Functions

    void Awake()
    {
        manager = GameObject.FindGameObjectWithTag(managerTag).GetComponent<PlayersManager>();

        manager.AddPlayer(this);
    }


    void Start()
    {
        //currentDamage = 0;
    }


    void Update()
    {
        // Clamp damage below max
        if(currentDamage > maxDamage){
            currentDamage = maxDamage;
        }

        // Update immunity timer
        if(immunityTimer > 0){
            isImmune = true;
            immunityTimer -= Time.deltaTime;
        }
        else{
            isImmune = false;
            immunityTimer = 0;
        }
    }


    // Public Functions
    
    public void SetRagdollState(bool ragdollState){
        // Update ragdoll flag state
        isRagdoll = ragdollState;

        // Triggers the ActiveRagdoll to go limp
        activeRagdoll.SetJointMotorsState(!ragdollState);
        
        // Change leg physic materials
        activeRagdoll.SetLegPhysicMaterial(ragdollState);
    }


    public bool OnBodyPartHit(Hittable bodyPart, Vector3 hitLocation, Vector3 hitRelativeVelocity, float hitDamage, float hitKnockbackMultiplier){
        // Apply damage to the player, then apply knockback to the body part that was hit
        // Returns whether the hit was successful (if player is not immune)

        if(!isImmune){
            currentDamage += hitDamage;

            float totalKnockbackMultiplier = hitKnockbackMultiplier * currentDamage.Map(0, 100, knockbackMultiplierAt0, knockbackMultiplierAt100);

            // Calculate the force required to set the new velocity, and multiply it by the total knockback multiplier
            Vector3 velocityChange = hitRelativeVelocity;
            bodyPart.rigidbody.AddForceAtPosition(velocityChange * totalKnockbackMultiplier, hitLocation, ForceMode.VelocityChange);
            //bodyPart.rigidbody.AddForce(velocityChange * totalKnockbackMultiplier, ForceMode.VelocityChange);

            TriggerImmunity();

            return true;
        }

        else{
            return false;
        }
    }


    public void TriggerImmunity(){
        // Makes player immune to damage and knockback for a set time, then makes them un-immune
        immunityTimer = hitImmunityDuration;
        isImmune = true;
    }



    // Private Functions

    
}
