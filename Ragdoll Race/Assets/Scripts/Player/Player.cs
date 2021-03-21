using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Component References")]
    public PlayersManager manager;
    public Rigidbody rb;
    public Collider coll;


    [Header("State Variables")]
    public bool isGrounded;
    public bool isDizzy;



    // Unity Functions
    void Start()
    {
        
    }


    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        isGrounded = true;
    }
    private void OnCollisionExit(Collision other)
    {
        isGrounded = false;
    }


    // Public Functions

    // Private Functions
}
