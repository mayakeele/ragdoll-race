using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Component References")]
    private Player thisPlayer;
    private Rigidbody rb;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;



    // Main Functions
    void Start()
    {
        thisPlayer = GetComponent<Player>();
        rb = GetComponent<Rigidbody>();
    }


    void Update()
    {
        
    }
}
