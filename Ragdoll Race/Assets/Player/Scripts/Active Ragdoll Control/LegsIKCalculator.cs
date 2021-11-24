using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DitzelGames.FastIK;

public class LegsIKCalculator : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private ActiveRagdoll activeRagdoll;
    [SerializeField] private Rigidbody pelvisRigidbody;
    [Space]
    [SerializeField] private FastIKFabric leftLegIK;
    [SerializeField] private FastIKFabric rightLegIK;
    [Space]
    [SerializeField] private Transform leftLegRoot;
    [SerializeField] private Transform rightLegRoot;


    [Header("Step Animation Properties")]

    [SerializeField] private float minDisplacementToMove;

    [Tooltip("Forward offset of the ground detecting ray as a function of the player's normalized speed.")]
    [SerializeField] private AnimationCurve stepVelocityOffset;

    [Tooltip("Duration of an entire step cycle, including both legs' movement and downtime, as a function of the player's normalized speed.")]
    [SerializeField] private AnimationCurve stepCycleLength;

    [Tooltip("Duration of a single step animation, from when a foot leaves the ground to when it touches back down, as a function of the player's normalized speed.")]
    [SerializeField] private AnimationCurve stepAnimationDuration;

    [Tooltip("Step height scalar as a function of the player's normalized speed.")]
    [SerializeField] private AnimationCurve stepAnimationMaxHeight;

    [Space]

    [Tooltip("Relative horizontal position of the IK target over time during one step. Position and time are normalized between 0 and 1")]
    [SerializeField] private AnimationCurve stepDistanceCurve;

    [Tooltip("Relative vertical position of the foot over time during one step. Position and time are normalized between 0 and 1")]
    [SerializeField] private AnimationCurve stepHeightCurve;


    [Header("Step Calculation Properties")]
    [SerializeField] private float standingFeetWidth;
    [SerializeField] private float minLegExtension;
    [SerializeField] private float maxLegExtension;
    [SerializeField] private LayerMask walkableLayers;
    

    
    // Private Variables

    private float currStepCycleLength;
    private float currStepVelocityOffset;
    private float currStepAnimationDuration;
    private float currStepAnimationMaxHeight;

    private bool useDynamicGait;
    private float timeSinceLastStep;
    private bool leftFlag;

    private bool leftLegActive = false;
    private bool rightLegActive = false;



    // Unity Functions
    
    void Start()
    {

    }


    void FixedUpdate()
    {
        timeSinceLastStep += Time.fixedDeltaTime;


        // Update leg movement parameters based on the player's speed

        float currSpeed = activeRagdoll.GetRelativeVelocity().magnitude;
        float currSpeedNormalized = currSpeed / activeRagdoll.player.controller.currMoveSpeedLimit;

        currStepCycleLength = stepCycleLength.Evaluate(currSpeedNormalized);
        currStepVelocityOffset = stepVelocityOffset.Evaluate(currSpeedNormalized);
        currStepAnimationDuration = stepAnimationDuration.Evaluate(currSpeedNormalized);
        currStepAnimationMaxHeight = stepAnimationMaxHeight.Evaluate(currSpeedNormalized);

        Debug.Log(currSpeedNormalized);


        // Only perform IK calculations if the player is not ragdolled
        if(!activeRagdoll.player.isRagdoll){

            // Move the current leg's IK bone if enough time has passed

            if(timeSinceLastStep >= currStepCycleLength / 2){
                timeSinceLastStep = 0;

                FastIKFabric currentLeg = leftFlag ? leftLegIK : rightLegIK;
                Transform currentTarget = currentLeg.Target;

                Vector3 desiredPosition = CastToGround(leftFlag);
                float displacementFromDefault = Vector3.Distance(currentTarget.position, desiredPosition);

                if(displacementFromDefault >= minDisplacementToMove){

                    StartCoroutine(MoveLegRelative(currentLeg, desiredPosition, leftFlag));
                }

                leftFlag = !leftFlag;
            }


            // Translate the inactive leg's target with the ground
            if(!leftLegActive){
                MoveTargetWithGround(leftLegIK.Target);
            }
            if(!rightLegActive){
                MoveTargetWithGround(rightLegIK.Target);
            }
        }
        
    }



    // Public Functions


    // Private Functions

    private Vector3 CastToGround(bool isLeft){
        // Casts a ray down and through the desired position to find solid ground

        // Calculate the horizontal and max possible vertical position of the foot
        Vector3 rayOrigin = isLeft ? leftLegRoot.position : rightLegRoot.position;
        rayOrigin += Vector3.down * minLegExtension;  
        rayOrigin += activeRagdoll.GetRelativeVelocity().ProjectHorizontalNormalized() * currStepVelocityOffset;

        float legExtensionRange = maxLegExtension - minLegExtension;

        // Cast a ray down from above the desired position to find solid ground
        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit groundHitInfo, legExtensionRange, walkableLayers)){
            // If solid ground is detected between the maximum and minimum leg extension, return the hit point
            return groundHitInfo.point;           
        }
        else{
            // If no ground is detected, fully extend leg
            return rayOrigin + (Vector3.down * legExtensionRange);
        }     
    }


    private IEnumerator MoveLeg(FastIKFabric leg, Vector3 newPosition, bool isLeft){
        // Moves the given leg along a path defined by direction to the new target and the step animation curve

        if(isLeft){ leftLegActive = true; } else{ rightLegActive = true; }

        Vector3 oldPosition = leg.Target.position;
        Vector3 totalDisplacement = newPosition - oldPosition;

        float timeGradient = 0;
        while (timeGradient <= 1){
            timeGradient = Mathf.Clamp01(timeGradient);

            float spaceGradient = stepDistanceCurve.Evaluate(timeGradient);

            Vector3 currentHeight = stepHeightCurve.Evaluate(spaceGradient) * currStepAnimationMaxHeight * Vector3.up;
            Vector3 currentDirection = spaceGradient * totalDisplacement;

            Vector3 currentPosition = oldPosition + currentDirection + currentHeight;
            leg.Target.position = currentPosition;

            timeGradient += Time.fixedDeltaTime / currStepAnimationDuration;

            yield return new WaitForFixedUpdate();
        }

        leg.Target.position = newPosition;

        if(isLeft){ leftLegActive = false; } else{ rightLegActive = false; }

        yield break;
    }

    private IEnumerator MoveLegRelative(FastIKFabric leg, Vector3 newWorldPosition, bool isLeft){
        // Moves the given leg from its old target position to a new target position, which moves and rotates with the player root

        if(isLeft){ leftLegActive = true; } else{ rightLegActive = true; }


        Transform playerTransform = activeRagdoll.player.rootForward;

        Vector3 oldRelativePosition = leg.Target.position - playerTransform.position;
        Vector3 newRelativePosition = newWorldPosition - playerTransform.position;

        Vector3 totalDisplacement = newRelativePosition - oldRelativePosition;


        float timeGradient = 0;
        while (timeGradient <= 1){
            timeGradient = Mathf.Clamp01(timeGradient);

            float spaceGradient = stepDistanceCurve.Evaluate(timeGradient);

            Vector3 currentVertical = stepHeightCurve.Evaluate(spaceGradient) * currStepAnimationMaxHeight * Vector3.up;
            Vector3 currentHorizontal = spaceGradient * totalDisplacement;

            Vector3 currentRelativePosition = oldRelativePosition + currentHorizontal + currentVertical;

            leg.Target.position = playerTransform.position + currentRelativePosition;

            timeGradient += Time.fixedDeltaTime / currStepAnimationDuration;

            yield return new WaitForFixedUpdate();
        }


        if(isLeft){ leftLegActive = false; } else{ rightLegActive = false; }

        yield break;
    }


    private FastIKFabric GetLeg(bool isLeft){
        return isLeft ? leftLegIK : rightLegIK;
    }


    private void MoveTargetWithGround(Transform target){
        target.transform.Translate(activeRagdoll.player.groundVelocity * Time.fixedDeltaTime, Space.World);
    }
}
