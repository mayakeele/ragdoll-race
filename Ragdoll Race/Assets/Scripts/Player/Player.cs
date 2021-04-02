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
    [SerializeField] private string managerTag = "PlayersManager";


    [Header("State Variables")]
    public bool isGrounded;
    public bool isRagdoll;
    private int numCollisions = 0;
    public bool isDizzy;



    // Unity Functions

    void Awake()
    {
        manager = GameObject.FindGameObjectWithTag(managerTag).GetComponent<PlayersManager>();

        manager.AddPlayer(this);
    }


    void Start()
    {
        
    }


    void Update()
    {
        
    }


    // Public Functions
    
    public void SetRagdollState(bool ragdollState){
        // Triggers the ActiveRagdoll to go limp, updates this player's state variable
        activeRagdoll.SetJointMotorsState(!ragdollState);
        isRagdoll = ragdollState;
    }


    // Private Functions
}
