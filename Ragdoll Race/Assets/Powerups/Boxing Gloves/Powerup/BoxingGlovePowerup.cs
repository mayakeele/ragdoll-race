using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxingGlovePowerup : Powerup
{
    [Header("References")]
    [SerializeField] private GameObject leftGlovePrefab;
    [SerializeField] private GameObject rightGlovePrefab;

    [Header("Boxing Glove Properties")]
    [SerializeField] private float powerupDuration;
    [SerializeField] private Vector3 leftOffset;
    [SerializeField] private Vector3 rightOffset;



    private GameObject leftGlove;
    private GameObject rightGlove;


    public override bool OnActivateInitial(){
        // Spawn left and right gloves on the player's hands, and create a fixed joint

        leftGlove = Instantiate(leftGlovePrefab);
        rightGlove = Instantiate(rightGlovePrefab);

        // Set gloves' Hitter components' attached player
        leftGlove.GetComponent<Hitter>().SetAttachedPlayer(attachedPlayer);
        rightGlove.GetComponent<Hitter>().SetAttachedPlayer(attachedPlayer);

        // Get references to player hand positions
        Transform leftHandTransform = attachedPlayer.activeRagdoll.leftArmOuterTransform;
        Transform rightHandTransform = attachedPlayer.activeRagdoll.rightArmOuterTransform;


        // Set glove transforms to match arms
        Vector3 leftPosition = leftHandTransform.TransformPoint(leftOffset);
        leftGlove.transform.SetPositionAndRotation(leftPosition, leftHandTransform.rotation);

        Vector3 rightPosition = rightHandTransform.TransformPoint(rightOffset);
        rightGlove.transform.SetPositionAndRotation(rightPosition, rightHandTransform.rotation);

        // Create joints to fix gloves to arms
        FixedJoint leftJoint = leftGlove.AddComponent<FixedJoint>();
        leftJoint.connectedBody = leftHandTransform.GetComponent<Rigidbody>();
        leftJoint.enableCollision = false;
        leftJoint.enablePreprocessing = false;

        FixedJoint rightJoint = rightGlove.AddComponent<FixedJoint>();
        rightJoint.connectedBody = rightHandTransform.GetComponent<Rigidbody>();
        rightJoint.enableCollision = false;
        rightJoint.enablePreprocessing = false;


        // Queue removal of the powerup after a while
        StartCoroutine(RemoveGlovesAfterWait());

        return true;
    }


    public override void OnRemove(){
        if(leftGlove) Destroy(leftGlove);
        if(rightGlove) Destroy(rightGlove);
    }


    private IEnumerator RemoveGlovesAfterWait(){
        // Removes the powerup and gloves items after a set amount of time
        // Also spawns VFX objects before destroying self

        yield return new WaitForSeconds(powerupDuration);

        // ~~~~~ Spawn VFX objects

        attachedPowerupManager.RemovePowerup(this);
    }
}
