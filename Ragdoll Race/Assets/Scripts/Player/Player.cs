using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Component References")]
    [HideInInspector] public PlayersManager manager;
    public ActiveRagdoll activeRagdoll;
    public Rigidbody rootRigidbody;
    public Collider coll;
    [SerializeField] private string managerTag = "PlayersManager";


    [Header("State Variables")]
    public bool isGrounded;
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


    // Private Functions
}
