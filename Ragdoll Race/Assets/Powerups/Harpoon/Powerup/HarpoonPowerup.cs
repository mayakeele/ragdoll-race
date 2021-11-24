using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarpoonPowerup : Powerup
{

    private enum HarpoonState{
        Ready,
        Launched,
        Hooked,
        Failed
    }


    [Header("References")]
    [SerializeField] private GameObject hookPrefab;
    [SerializeField] private GameObject ropePrefab;
    [SerializeField] private GameObject beltPrefab;


    [Header("Spawn Properties")]
    [SerializeField] private float powerupDuration;
    [SerializeField] private Vector3 hookSpawnOffset;
    [Space]
    [SerializeField] private float harpoonRangeMax;
    [SerializeField] private float harpoonRangeMin;
    [SerializeField] private float harpoonAngularRange;



    bool isLaunching;
    bool isHooked;
    bool isReeling;
    HarpoonState currentState;

    private Transform hookTransform;
    private HarpoonHookEntity hookEntity;
    private Transform beltTransform;
    private Transform ropeTransform;

    private Player targetPlayer;



    public override bool OnActivateInitial()
    {
        // Only activate the harpoon if a player is within range
        if(IsPlayerInRange(out targetPlayer)){
            // Spawn each piece of the harpoon and send the hook flying
            LaunchHarpoon();

            return true;
        }
        else{
            return false;
        }
    }


    public override void OnActivateContinued()
    {
        // Any further action button presses will cause the harpoon to reel in
        isReeling = true;
    }

    public override void OnActivateButtonReleased()
    {
        // When activation button is released, stop reeling in
        isReeling = false;
    }



    public void OnHookSuccess(){
        // 
        currentState = HarpoonState.Hooked;
    }

    public void OnHookFailure(){
        // 
        currentState = HarpoonState.Failed;
    }



    private void SpawnBelt(){
        // Spawns a belt prefab and attaches it to the player's torso (visual only)
        RemoveHarpoonParts(false,true,false);

        beltTransform = Instantiate(beltPrefab).transform;
        beltTransform.SetParent(attachedPlayer.activeRagdoll.torsoLowerTransform);
        beltTransform.localPosition = Vector3.zero;
        beltTransform.localRotation = Quaternion.identity;
    }


    private void SpawnHook(){
        // Spawns a hook prefab in front of the player's torso and turns it to face the target player
        RemoveHarpoonParts(true,false,false);

        Vector3 spawnPosition = attachedPlayer.activeRagdoll.torsoLowerTransform.TransformPoint(hookSpawnOffset);
        Quaternion spawnRotation = Quaternion.LookRotation(targetPlayer.activeRagdoll.torsoLowerTransform.position - spawnPosition, Vector3.up);

        hookTransform = SpawnedEntity.SpawnEntityForPlayer(hookPrefab, attachedPlayer, spawnPosition, spawnRotation).transform;
        hookEntity = hookTransform.GetComponent<HarpoonHookEntity>();
        hookEntity.attachedPowerup = this;
    }



    private void LaunchHarpoon(){
        // Spawns in harpoon components and gives the hook some velocity

        currentState = HarpoonState.Launched;
        isLaunching = true;
        isReeling = false;
        isHooked = false;

        SpawnHook();
        SpawnBelt();

        hookEntity.Launch();
    }


    private void RemoveHarpoonParts(bool destroyHook = true, bool destroyBelt = true, bool destroyRope = true){
        if(destroyHook && hookTransform) { Destroy(hookTransform); hookTransform = null; }
        if(destroyBelt && beltTransform) { Destroy(beltTransform); beltTransform = null; }
        if(destroyRope && ropeTransform) { Destroy(ropeTransform); ropeTransform = null; }    
    }



    private bool IsPlayerInRange(out Player closestPlayer){
        // Returns whether any player's torso is within lateral and angular range of the harpoon, and if true, the closest player

        List<Player> allOtherPlayers = attachedPlayer.manager.GetAllPlayers(attachedPlayer);

        closestPlayer = null;
        float closestDist = float.MaxValue;

        // Iterate over every player besides the powerup user
        foreach(Player otherPlayer in allOtherPlayers){

            Vector3 toOther = otherPlayer.activeRagdoll.torsoLowerTransform.position - attachedPlayer.activeRagdoll.torsoLowerTransform.position;

            float dist = toOther.magnitude;
            float angularDist = Vector3.Angle(attachedPlayer.rootForward.forward, toOther);

            if(dist <= harpoonRangeMax && dist >= harpoonRangeMin && angularDist <= harpoonAngularRange && dist < closestDist){
                closestDist = dist;
                closestPlayer = otherPlayer;
            }
        }

        return (closestPlayer != null);
    }
}
