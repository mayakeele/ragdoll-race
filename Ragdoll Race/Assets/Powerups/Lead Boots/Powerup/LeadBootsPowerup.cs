using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeadBootsPowerup : Powerup
{
    [Header("References")]
    [SerializeField] private GameObject leftBootPrefab;
    [SerializeField] private GameObject rightBootPrefab;

    [Header("Boot Properties")]
    [SerializeField] private float powerupDuration;
    [SerializeField] private Vector3 leftOffset;
    [SerializeField] private Vector3 rightOffset;



    private GameObject leftBoot;
    private GameObject rightBoot;


    public override bool OnActivateInitial(){
        // Spawn left and right boots on the player's legs, and create a fixed joint

        leftBoot = Instantiate(leftBootPrefab);
        rightBoot = Instantiate(rightBootPrefab);

        // Set boots' Hitter components' attached player
        leftBoot.GetComponent<Hitter>().SetAttachedPlayer(attachedPlayer);
        rightBoot.GetComponent<Hitter>().SetAttachedPlayer(attachedPlayer);

        // Get references to player leg transforms
        Transform leftFootTransform = attachedPlayer.activeRagdoll.leftLegOuterTransform;
        Transform rightFootTransform = attachedPlayer.activeRagdoll.rightLegOuterTransform;


        // Set boot transforms to match legs
        Vector3 leftPosition = leftFootTransform.TransformPoint(leftOffset);
        leftBoot.transform.SetPositionAndRotation(leftPosition, leftFootTransform.rotation);

        Vector3 rightPosition = rightFootTransform.TransformPoint(rightOffset);
        rightBoot.transform.SetPositionAndRotation(rightPosition, rightFootTransform.rotation);

        // Create joints to fix boots to legs
        FixedJoint leftJoint = leftBoot.AddComponent<FixedJoint>();
        leftJoint.connectedBody = leftFootTransform.GetComponent<Rigidbody>();
        leftJoint.enableCollision = false;
        leftJoint.enablePreprocessing = false;

        FixedJoint rightJoint = rightBoot.AddComponent<FixedJoint>();
        rightJoint.connectedBody = rightFootTransform.GetComponent<Rigidbody>();
        rightJoint.enableCollision = false;
        rightJoint.enablePreprocessing = false;


        // Queue removal of the powerup after a while
        StartCoroutine(RemoveBootsAfterWait());

        return true;
    }


    public override void OnRemove(){
        if(leftBoot) Destroy(leftBoot);
        if(rightBoot) Destroy(rightBoot);
    }


    private IEnumerator RemoveBootsAfterWait(){
        // Removes the powerup and gloves items after a set amount of time
        // Also spawns VFX objects before destroying self

        yield return new WaitForSeconds(powerupDuration);

        // ~~~~~ Spawn VFX objects

        attachedPowerupManager.RemovePowerup(this);
    }
}
