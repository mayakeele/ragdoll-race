using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearTrapPowerup : Powerup
{
    [Header("Bear Trap Spawning Properties")]
    [SerializeField] private int numTraps = 1;
    [Space]
    [SerializeField] private GameObject trapPrefab;

    

    public override void OnActivate(){
        // If the player is grounded, attach a bear trap prefab to that ground

        Player attachedPlayer = attachedPowerupManager.player;

        if(attachedPlayer.isGrounded && numTraps > 0){
            Vector3 position = attachedPlayer.groundPosition;
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, attachedPlayer.groundNormal);
            Transform parent = attachedPlayer.groundTransform;

            if(position != null && rotation != null && parent) SpawnTrap(position, rotation, parent);
        }

        if(numTraps <= 0){
            attachedPowerupManager.RemovePowerup();
        }
    }


    private void SpawnTrap(Vector3 position, Quaternion rotation, Transform parent){
        // Create mine prefab at position and rotation of the ground, make child of ground
        
        // Spawn a mine prefab at the desired location
        GameObject newTrap = SpawnedItem.SpawnItemForPlayer(trapPrefab, attachedPowerupManager.player, position, rotation, parent);

        numTraps--;
    }
}
