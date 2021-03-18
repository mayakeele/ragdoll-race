using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Component References")]
    public Rigidbody rb;
    public CameraController cameraController;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
