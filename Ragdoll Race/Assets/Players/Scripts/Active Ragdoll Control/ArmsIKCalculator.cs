using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DitzelGames.FastIK;

public class ArmsIKCalculator : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private ActiveRagdoll activeRagdoll;
    [Space]
    [SerializeField] private FastIKFabric leftArmIK;
    [SerializeField] private FastIKFabric rightArmIK;
    [Space]
    [SerializeField] private Transform leftArmRoot;
    [SerializeField] private Transform rightArmRoot;

    [Header("Punching Properties")]
    [SerializeField] private float punchRecoveryDuration;

    [Header("Climbing Properties")]
    [SerializeField] private LayerMask climbableLayers;



    // Unity Functions
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        if(!activeRagdoll.player.isRagdoll){

        }
    }
}
