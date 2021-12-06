using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVFX : MonoBehaviour
{
    [Header("Component References")]
    public Player player;
    

    [Space]

    [Header("Ground Indicator")]
    [SerializeField] private Transform groundIndicator;
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private float groundDetectionRadius;
    [SerializeField] private float groundDetectionDistance;
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




    void Update()
    {
        groundIndicator.position = DetectGroundSphereCast();
    }




    private Vector3 DetectGroundSphereCast(){
        // Spherecasts down from the player's feet to find ground to place the indicator
        // Returns a point at the detected height centered below the player's feet
        Vector3 startingPosition = player.activeRagdoll.GetLowerFootCentered();

        if(Physics.SphereCast(startingPosition, groundDetectionRadius, Vector3.down, out RaycastHit hitInfo, groundDetectionDistance, groundLayers)){
            Vector3 detectedPosition = hitInfo.point;
            return new Vector3(startingPosition.x, detectedPosition.y, startingPosition.z);
        }
        else{
            return startingPosition;
        }
    }


    public void SpawnAirJumpVFX(Vector3 position){
        GameObject.Instantiate(airJumpVFX, position + (airJumpFootOffset * Vector3.down), Quaternion.identity);
    }

}
