using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearTrapPowerup : Powerup
{
    [Header("Bear Trap Spawning Properties")]
    [SerializeField] private int numTraps = 1;
    [Space]
    [SerializeField] private GameObject trapPrefab;

    

    public override bool OnActivateInitial(){
        // If the player is grounded, attach a bear trap prefab to that ground

        Player attachedPlayer = attachedPowerupManager.player;
        bool placementSuccessful = false;

        if(attachedPlayer.isGrounded && numTraps > 0){
            Vector3 position = attachedPlayer.groundPosition;
            Vector3 forwardDirection = Vector3.ProjectOnPlane(attachedPlayer.rootForward.position, attachedPlayer.groundNormal);
            Quaternion rotation = Quaternion.LookRotation(forwardDirection, attachedPlayer.groundNormal);
            Transform parent = attachedPlayer.groundTransform;

            if(position != null && rotation != null && parent) SpawnTrap(position, rotation, parent);

            placementSuccessful = true;
        }

        if(numTraps <= 0){
            attachedPowerupManager.RemovePowerup(this);
        }

        return placementSuccessful;
    }

    public override void OnActivateContinued()
    {
        OnActivateInitial();
    }


    private void SpawnTrap(Vector3 position, Quaternion rotation, Transform parent){
        // Create mine prefab at position and rotation of the ground, make child of ground
        
        // Spawn a mine prefab at the desired location
        GameObject newTrap = SpawnedEntity.SpawnEntityForPlayer(trapPrefab, attachedPowerupManager.player, position, rotation, parent);

        numTraps--;
    }
}
