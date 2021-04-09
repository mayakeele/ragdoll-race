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


    [Header("State Variables")]
    public float currentDamage = 0;
    public bool isGrounded;
    public bool isRagdoll;
    public bool isDizzy;



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


    public void OnBodyPartHit(Hittable bodyPart, Vector3 hitLocation, Vector3 hitImpulse, float hitDamage, float hitKnockbackMultiplier){
        // Apply damage to the player, then apply knockback to the body part that was hit
        currentDamage += hitDamage;

        float damageKnockbackMultiplier = currentDamage.Map(0, 100, knockbackMultiplierAt0, knockbackMultiplierAt100);
        
        Vector3 knockbackImpulse = damageKnockbackMultiplier * hitKnockbackMultiplier * hitImpulse;
        bodyPart.GetComponent<Rigidbody>().AddForceAtPosition(knockbackImpulse, hitLocation, ForceMode.Impulse);
    }



    // Private Functions

}
