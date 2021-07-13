using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BanhammerPowerup : Powerup
{
    [Header("References")]
    [SerializeField] private GameObject banhammerPrefab;


    [Header("Banhammer Properties")]
    //[SerializeField] private float powerupDuration;
    [SerializeField] private Vector3 placementOffset;


    [Header("Effects")]
    [SerializeField] private GameObject spawnEffect;
    [SerializeField] private GameObject removalEffect;


    private GameObject banhammerObject;
    private BanhammerEntity banhammerEntity;
    private FixedJoint joint;



    public override bool OnActivateInitial(){
        SpawnHammer();
        return true;
    }


    public override void OnRemove()
    {
        RemoveHammer();
    }



    public void SpawnHammer(){
        // Spawns in a banhammer and attaches it to the player's right hand, also creates an effect
        Transform handTransform = attachedPlayer.activeRagdoll.rightArmOuterTransform;

        banhammerObject = SpawnedEntity.SpawnEntityForPlayer(banhammerPrefab, attachedPlayer, handTransform.TransformPoint(placementOffset), handTransform.rotation);
        banhammerEntity = banhammerObject.GetComponentInChildren<BanhammerEntity>();

        joint = banhammerEntity.rigidbody.gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = handTransform.GetComponent<Rigidbody>();

        banhammerEntity.GetComponent<Hitter>().SetAttachedPlayer(attachedPlayer);

        if(spawnEffect) Instantiate(spawnEffect, banhammerObject.transform.position, banhammerObject.transform.rotation);
    }


    public void RemoveHammer(){
        // Destroys the hammer object and spawns an effect
        Destroy(banhammerObject);

        banhammerObject = null;
        banhammerEntity = null;
        joint = null;

        if(removalEffect) Instantiate(removalEffect, banhammerObject.transform.position, banhammerObject.transform.rotation);
    }

}
