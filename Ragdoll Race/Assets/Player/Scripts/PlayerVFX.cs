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
    [SerializeField] private Material groundIndicatorMaterial;
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


    private void Start()
    {
        groundIndicatorMaterial.SetColor("_Color", player.skinnedMeshRenderer.material.color);
    }

    void Update()
    {
        // if grounded, snap to stored ground position and give it a more discreet look
        // If in the air, scale by distance to the ground and make more obvious
        // offset from the ground a bit
        // have it follow player root instead of feet average

        PlaceGroundIndicator();
        
    }




    private void PlaceGroundIndicator(){
        // Spherecasts down from the player's feet to find ground to place the indicator
        // Returns a point at the detected height centered below the player's feet

        // When grounded, simply place on the ground at full expansion
        if(player.isGrounded){
            groundIndicator.gameObject.SetActive(true);

            groundIndicator.position = player.groundPosition + (Vector3.up * groundIndicatorOffset);
        }
        // In the air, scale the indicator down to a certain point, then don't display at all
        else{
            Vector3 startingPosition = player.activeRagdoll.GetLowerFootCenteredRoot() + (groundDetectionRadius * Vector3.up);

            if(Physics.SphereCast(startingPosition, groundDetectionRadius, Vector3.down, out RaycastHit hitInfo, groundDetectionDistance, groundLayers)){
                groundIndicator.gameObject.SetActive(true);

                float detectedHeight = hitInfo.point.y;
                groundIndicator.position = new Vector3(player.rootRigidbody.position.x, detectedHeight + groundIndicatorOffset, player.rootRigidbody.position.z);
            }
            else{
                groundIndicator.gameObject.SetActive(false);
            }
        }    
    }


    public void SpawnAirJumpVFX(Vector3 position){
        GameObject.Instantiate(airJumpVFX, position + (airJumpFootOffset * Vector3.down), Quaternion.identity);
    }

}
