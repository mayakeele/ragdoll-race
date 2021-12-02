using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Component References")]
    [HideInInspector] public PlayersManager manager;
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public PlayerController controller;
    public PlayerPowerupManager powerupManager;
    public ActiveRagdoll activeRagdoll;
    public PlayerVFX vfx;
    public AudioSource audioSource;
    public Rigidbody rootRigidbody;
    public Transform rootForward;
    
    [SerializeField] private string managerTag = "PlayersManager";
    private PlayerInput playerInput;


    [Header("Damage & Knockback Properties")]
    [SerializeField] private AnimationCurve knockbackMultiplierCurve;
    [Space]
    [SerializeField] private float maxDamage;
    [SerializeField] private float hitImmunityDuration;
    [Space]
    [SerializeField] private AnimationCurve knockbackAngleCurve;


    [Header("State Variables")]
    public float currentDamage = 0;
    public bool isGrounded;
    public bool isRagdoll;
    private bool forceRagdollState;
    private float forceRagdollTimer;
    public bool autoGetup;

    

    [Header("Hit Tracking")]
    private Player lastPlayerHitBy;
    public bool isImmune;
    private float immunityTimer;
    private bool knockedOutThisFrame;



    [Header("Grounded Variables")]
    [HideInInspector] public Transform groundTransform = null;
    [HideInInspector] public Rigidbody groundRigidbody = null;
    [HideInInspector] public Vector3 groundPosition = Vector3.zero;
    [HideInInspector] public Vector3 groundVelocity = Vector3.zero;
    [HideInInspector] public Vector3 groundNormal = Vector3.zero;



    // Unity Functions

    void Awake()
    {

        playerInput = GetComponentInChildren<PlayerInput>();
        manager = GameObject.FindGameObjectWithTag(managerTag).GetComponent<PlayersManager>();

        int playerIndex = playerInput.playerIndex;
        manager.AddPlayer(this);

        skinnedMeshRenderer.material = manager.GetPlayerMaterial(playerIndex);
    }


    void Start()
    {
        //currentDamage = 0;
        RespawnAtPosition(manager.spawnTransform.position);
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


        // Update forced ragdoll timer
        if(forceRagdollTimer > 0){
            forceRagdollState = true;
            forceRagdollTimer -= Time.deltaTime;
        }
        else{
            // Re-enable control on the first frame the force timer runs out. Otherwise 
            if(forceRagdollState == true && autoGetup){
                SetRagdollState(false);
            }

            forceRagdollState = false;
            forceRagdollTimer = 0;
        }


        // Respawn if below y -10
        if(rootRigidbody.transform.position.y < -30){
            RespawnAtPosition(manager.spawnTransform.position);
        }
    }


    void FixedUpdate()
    {
        knockedOutThisFrame = false;
    }


    // Public Functions
    
    public void TrySetRagdollState(bool ragdollState){
        // Will attempt to set the ragdoll state to the given value, but will only do so successfully if the ragdoll state is not currently forced
        if(forceRagdollState == false){
            SetRagdollState(ragdollState);
        }
    }
    


    public bool OnBodyPartHit(Hittable bodyPart, Vector3 hitLocation, Vector3 hitRelativeVelocity, float hitDamage, float hitKnockbackMultiplier, float ragdollDuration, Player attacker = null){
        // Apply damage to the player, then apply knockback to the body part that was hit
        // Returns whether the hit was successful (if player is not immune)

        if(!isImmune){
            currentDamage += hitDamage;

            float totalKnockbackMultiplier = hitKnockbackMultiplier * knockbackMultiplierCurve.Evaluate(currentDamage);

            // Calculate the force required to set the new velocity, and multiply it by the total knockback multiplier
            Vector3 velocityChange = hitRelativeVelocity;
            bodyPart.rigidbody.AddForceAtPosition(velocityChange * totalKnockbackMultiplier, hitLocation, ForceMode.VelocityChange);
            //bodyPart.rigidbody.AddForce(velocityChange * totalKnockbackMultiplier, ForceMode.VelocityChange);

            TriggerImmunity();

            ForceRagdoll(ragdollDuration);

            return true;
        }

        else{
            return false;
        }
    }


    public void TriggerImmunity(){
        // Makes player immune to damage and knockback for a set time
        immunityTimer = hitImmunityDuration;
        isImmune = true;
    }

    public void ForceRagdoll(float forceRagdollDuration){
        // Force player into ragdoll state for a set time
        forceRagdollTimer = forceRagdollDuration;
        forceRagdollState = true;
        SetRagdollState(true);
    }


    public void TryKnockout(GameObject prefabToSpawn = null){
        // Tells the player manager that this player has been killed. Passes on the given prefab to spawn. 
        // If no prefab is given, passes null, PlayerManager will instantiate default instead.
        // All actions should be done on the PlayerManager; this function should not take any action beyond notifying the manager

        if(!knockedOutThisFrame){         
            knockedOutThisFrame = true;
            manager.KnockoutPlayer(this, rootRigidbody.position, prefabToSpawn);
        }    
    }


    public void RespawnAtPosition(Vector3 spawnPosition){
        // Respawns the player in the given position, resetting their damage, ragdoll, powerups and physics

        powerupManager.RemoveAllPowerups();

        currentDamage = 0;
        TriggerImmunity();

        SetRagdollState(false);
        activeRagdoll.MoveToPosition(spawnPosition);
        
        controller.ResetAirJumpCount();
    }



    // Private Functions

    private void SetRagdollState(bool ragdollState){
        // Update ragdoll flag state
        isRagdoll = ragdollState;

        // Triggers the ActiveRagdoll to go limp
        activeRagdoll.SetJointMotorsState(!ragdollState);
        
        // Change leg physic materials
        activeRagdoll.SetLegPhysicMaterial(ragdollState);
    }

}
