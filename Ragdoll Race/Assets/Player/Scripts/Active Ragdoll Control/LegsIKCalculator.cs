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
    [SerializeField] private AnimationCurve stepVelocityOffset;
    [SerializeField] private AnimationCurve stepCycleLength;
    [SerializeField] private AnimationCurve stepAnimationDuration;
    [SerializeField] private AnimationCurve stepAnimationMaxHeight;
    [Space]
    [SerializeField] private AnimationCurve stepDistanceCurve;
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
    private bool leftLegMoving;

    private Vector3 leftFootAnchor;
    private Vector3 rightFootAnchor;



    // Unity Functions
    
    void Start()
    {

    }


    void FixedUpdate()
    {
        timeSinceLastStep += Time.fixedDeltaTime;


        // Update leg movement parameters based on the player's speed

        float currSpeed = pelvisRigidbody.velocity.magnitude;

        currStepCycleLength = stepCycleLength.Evaluate(currSpeed);
        currStepVelocityOffset = stepVelocityOffset.Evaluate(currSpeed);
        currStepAnimationDuration = stepAnimationDuration.Evaluate(currSpeed);
        currStepAnimationMaxHeight = stepAnimationMaxHeight.Evaluate(currSpeed);


        // Only perform IK calculations if the player is not ragdolled
        if(!activeRagdoll.player.isRagdoll){

            // Move the current leg's IK bone if enough time has passed

            if(timeSinceLastStep >= currStepCycleLength / 2){
                timeSinceLastStep = 0;

                FastIKFabric currentLeg = leftLegMoving ? leftLegIK : rightLegIK;
                Transform currentTarget = currentLeg.Target;

                Vector3 desiredPosition = CastToGround(leftLegMoving);
                float displacementFromDefault = Vector3.Distance(currentTarget.position, desiredPosition);

                if(displacementFromDefault >= minDisplacementToMove){
                    StartCoroutine(MoveLegRelative(currentLeg, desiredPosition));
                    if(leftLegMoving){ leftFootAnchor = desiredPosition; }  else{ rightFootAnchor = desiredPosition; }
                }

                leftLegMoving = !leftLegMoving;
            }

        }
        
    }



    // Public Functions

    public List<Vector3> GetFootAnchors(){
        List<Vector3> anchors = new List<Vector3>(2);
        anchors.Add(leftFootAnchor);
        anchors.Add(rightFootAnchor);
        return anchors;
    }



    // Private Functions

    private Vector3 CastToGround(bool isLeft){
        // Casts a ray down and through the desired position to find solid ground

        // Calculate the horizontal and max possible vertical position of the foot
        Vector3 rayOrigin = isLeft ? leftLegRoot.position : rightLegRoot.position;
        rayOrigin += Vector3.down * minLegExtension;  
        rayOrigin += pelvisRigidbody.velocity.ProjectHorizontal() * currStepVelocityOffset;

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


    private IEnumerator MoveLeg(FastIKFabric leg, Vector3 newPosition){
        // Moves the given leg along a path defined by direction to the new target and the step animation curve

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

        yield break;
    }

    private IEnumerator MoveLegRelative(FastIKFabric leg, Vector3 newWorldPosition){
        // Moves the given leg from its old target position to a new target position, which moves and rotates with the player root

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

        //leg.Target.position = newPosition;

        yield break;
    }


    private FastIKFabric GetLeg(bool isLeft){
        return isLeft ? leftLegIK : rightLegIK;
    }

}