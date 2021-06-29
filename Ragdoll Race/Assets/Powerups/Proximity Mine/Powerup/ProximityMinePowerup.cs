using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityMinePowerup : Powerup
{
    [Header("Mine Spawning Properties")]
    [SerializeField] private int numMines = 2;
    //[SerializeField] private float spawnWaitDuration;
    //[SerializeField] private LayerMask placeableLayers = LayerMask.GetMask("StaticTerrain", "DynamicTerrain");
    [Space]
    [SerializeField] private GameObject minePrefab;

    

    public override bool OnActivateInitial(){
        // If the player is grounded, attach a proximity mine prefab on that ground

        Player attachedPlayer = attachedPowerupManager.player;
        bool placementSuccessful = false;

        if(attachedPlayer.isGrounded && numMines > 0){
            Vector3 position = attachedPlayer.groundPosition;
            Vector3 forwardDirection = Vector3.ProjectOnPlane(attachedPlayer.rootForward.position, attachedPlayer.groundNormal);
            Quaternion rotation = Quaternion.LookRotation(forwardDirection, attachedPlayer.groundNormal);
            Transform parent = attachedPlayer.groundTransform;

            if(position != null && rotation != null && parent) {
                SpawnMine(position, rotation, parent);
                placementSuccessful = true;
            }
        }

        if(numMines <= 0){
            attachedPowerupManager.RemovePowerup(this);
        }

        return placementSuccessful;
    }
    
    public override void OnActivateContinued()
    {
        OnActivateInitial();
    }


    private void SpawnMine(Vector3 position, Quaternion rotation, Transform parent){
        // Create mine prefab at position and rotation of the ground, make child of ground
        
        //GameObject newMine = GameObject.Instantiate(minePrefab, position, rotation, parent);
        // Spawn a mine prefab at the desired location
        GameObject newMine = SpawnedItem.SpawnItemForPlayer(minePrefab, attachedPowerupManager.player, position, rotation, parent);

        numMines--;

    }
}
