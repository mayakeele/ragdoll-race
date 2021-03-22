using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveRagdoll : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private Player player;
    [SerializeField] private Rigidbody pelvisRigidbody;


    [Header("Detection Settings")]
    [SerializeField] private LayerMask walkableLayers;


    [Header("Body Settings")]
    [SerializeField] private float targetPelvisHeight;


    [Header("Force Settings")]
    [SerializeField] private float torsoFollowForce;
    [SerializeField] private float headFollowForce;
    [SerializeField] private float armsFollowForce;
    [SerializeField] private float legsFollowForce;


    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
