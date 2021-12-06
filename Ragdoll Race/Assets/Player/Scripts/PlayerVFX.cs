using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVFX : MonoBehaviour
{
    [Header("Component References")]
    public Player player;
    

    [Space]

    [Header("Ground Indicator")]
    [SerializeField] private Material groundIndicator;
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private float groundDetectionRadius;
    [Space]
    [SerializeField] private float groundIndicatorSize;
    [SerializeField] private float groundIndicatorOffset;

    [Space]

    [Header("KO VFX")]
    [SerializeField] private GameObject defaultKOVFX;

    [Header("Jump VFX")]
    [SerializeField] private GameObject airJumpVFX;
    [SerializeField] private float airJumpFootOffset = 0.2f;

    //[Header("Running VFX")]

    //[Header("Attack VFX")]

    //[Header("Damage VFX")]

    //[Header("Trail VFX")]


    private void DetectGround(){

    }


    public void SpawnAirJumpVFX(Vector3 position){
        GameObject.Instantiate(airJumpVFX, position + (airJumpFootOffset * Vector3.down), Quaternion.identity);
    }

}
