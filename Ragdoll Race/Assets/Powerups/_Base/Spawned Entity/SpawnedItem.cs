using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnedItem : MonoBehaviour
{
    private Player attachedPlayer;


    public void SetAttachedPlayer(Player player){
        attachedPlayer = player;
    }

    public Player GetAttachedPlayer(){
        return attachedPlayer;
    }




    public static GameObject SpawnItemForPlayer(GameObject item, Player player){
        // Instantiates the given prefab, and attaches a player
        GameObject itemGameobject = GameObject.Instantiate(item);

        SpawnedItem spawnedItem = itemGameobject.GetComponentInChildren<SpawnedItem>();
        if(spawnedItem) spawnedItem.SetAttachedPlayer(player);

        return itemGameobject;
    }

    public static GameObject SpawnItemForPlayer(GameObject item, Player player, Vector3 position, Quaternion rotation){
        // Instantiates the given prefab at location, and attaches a player
        GameObject itemGameobject = GameObject.Instantiate(item, position, rotation);

        SpawnedItem spawnedItem = itemGameobject.GetComponentInChildren<SpawnedItem>();
        if(spawnedItem) spawnedItem.SetAttachedPlayer(player);

        return itemGameobject;
    }

    public static GameObject SpawnItemForPlayer(GameObject item, Player player, Vector3 position, Quaternion rotation, Transform parent){
        // Instantiates the given prefab at location and with parent, and attaches a player
        GameObject itemGameobject = GameObject.Instantiate(item, position, rotation, parent);

        SpawnedItem spawnedItem = itemGameobject.GetComponentInChildren<SpawnedItem>();
        if(spawnedItem) spawnedItem.SetAttachedPlayer(player);

        return itemGameobject;
    }
}
