using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnedEntity : MonoBehaviour
{
    private Player attachedPlayer;


    public void SetAttachedPlayer(Player player){
        attachedPlayer = player;
    }

    public Player GetAttachedPlayer(){
        return attachedPlayer;
    }




    public static GameObject SpawnEntityForPlayer(GameObject entity, Player player){
        // Instantiates the given prefab, and attaches a player
        GameObject entityGameobject = GameObject.Instantiate(entity);

        SpawnedEntity spawnedEntity = entityGameobject.GetComponentInChildren<SpawnedEntity>();
        if(spawnedEntity) spawnedEntity.SetAttachedPlayer(player);

        return entityGameobject;
    }

    public static GameObject SpawnEntityForPlayer(GameObject entity, Player player, Vector3 position, Quaternion rotation){
        // Instantiates the given prefab at location, and attaches a player
        GameObject entityGameobject = GameObject.Instantiate(entity, position, rotation);

        SpawnedEntity spawnedEntity = entityGameobject.GetComponentInChildren<SpawnedEntity>();
        if(spawnedEntity) spawnedEntity.SetAttachedPlayer(player);

        return entityGameobject;
    }

    public static GameObject SpawnEntityForPlayer(GameObject entity, Player player, Vector3 position, Quaternion rotation, Transform parent){
        // Instantiates the given prefab at location and with parent, and attaches a player
        GameObject entityGameobject = GameObject.Instantiate(entity, position, rotation, parent);

        SpawnedEntity spawnedEntity = entityGameobject.GetComponentInChildren<SpawnedEntity>();
        if(spawnedEntity) spawnedEntity.SetAttachedPlayer(player);

        return entityGameobject;
    }
}
