using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Component References")]
    [HideInInspector] public PlayersManager manager;
    public Rigidbody rb;
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
        isGrounded = (numCollisions > 0);
    }


    private void OnCollisionEnter(Collision other)
    {
        numCollisions++;
        isGrounded = true;
    }
    private void OnCollisionExit(Collision other)
    {
        numCollisions--;
        isGrounded = false;
    }


    // Public Functions


    // Private Functions
}
