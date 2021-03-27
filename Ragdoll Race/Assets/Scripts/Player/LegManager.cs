using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DitzelGames.FastIK;

public class LegManager : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private ActiveRagdoll activeRagdoll;
    [SerializeField] private Rigidbody pelvisRigidbody;
    [Space]
    [SerializeField] private FastIKFabric leftLegIK;
    [SerializeField] private FastIKFabric rightLegIK;
    [Space]
    [SerializeField] private ConfigurableJoint leftUpperJoint;
    [SerializeField] private ConfigurableJoint rightUpperJoint;
    [SerializeField] private ConfigurableJoint leftLowerJoint;
    [SerializeField] private ConfigurableJoint rightLowerJoint;


    [Header("Movement Properties")]
    [SerializeField] private float movingStepOffset;
    [SerializeField] private float minDisplacementToMove;


    [Header("Step Animation Properties")]
    [SerializeField] private float stepCycleLength;
    [SerializeField] private float stepAnimationDuration;
    [SerializeField] private float stepAnimationHeight;
    [SerializeField] private AnimationCurve stepHeightCurve;


    [Header("Step Calculation Properties")]
    [SerializeField] private float standingFeetWidth;
    [SerializeField] private float minLegLength;
    [SerializeField] private float maxLegLength;
    [SerializeField] private LayerMask walkableLayers;
    

    
    // Private Variables

    private bool useDynamicGait;
    private float timeSinceLastStep;
    private bool leftLegMoving;



    // Unity Functions
    
    void Start()
    {

    }


    void FixedUpdate()
    {
        timeSinceLastStep += Time.fixedDeltaTime;

        // Move the current leg IK bones after its alotted time
        if(timeSinceLastStep >= stepCycleLength / 2){
            timeSinceLastStep = 0;

            FastIKFabric currentLeg = leftLegMoving ? leftLegIK : rightLegIK;
            Transform currentTarget = currentLeg.Target;

            Vector3 desiredPosition = CastToGround(leftLegMoving, out Vector3 groundNormal);
            float displacementFromDefault = Vector3.Distance(currentTarget.position, desiredPosition);

            if(displacementFromDefault >= minDisplacementToMove){
                StartCoroutine(MoveLeg(currentLeg, desiredPosition, groundNormal));
            }

            leftLegMoving = !leftLegMoving;
        }

        
        // Update position and rotation targets for the physical legs
        // save for later:  poseTarget = Quaternion.Inverse(leftLowerJoint.transform.parent.rotation) * GetLegJointTargetRotation(true, true);

        leftLowerJoint.SetTargetRotation(GetLegJointTargetRotation(true, true), Quaternion.identity);
        leftUpperJoint.SetTargetRotation(GetLegJointTargetRotation(true, false), Quaternion.identity);

        rightLowerJoint.SetTargetRotation(GetLegJointTargetRotation(false, true), Quaternion.identity);
        rightUpperJoint.SetTargetRotation(GetLegJointTargetRotation(false, false), Quaternion.identity);
        
    }



    // Private Functions

    private Vector3 CastToGround(bool isLeft, out Vector3 groundNormal){
        // Casts a ray down and through the desired position to find solid ground

        // Calculate the horizontal and max possible vertical position of the foot
        Vector3 rayOrigin = pelvisRigidbody.worldCenterOfMass;
        rayOrigin += (pelvisRigidbody.transform.right.ProjectHorizontal() * (standingFeetWidth/2) * (isLeft ? -1 : 1));
        rayOrigin += pelvisRigidbody.velocity.ProjectHorizontal() * movingStepOffset;
        rayOrigin += Vector3.down * minLegLength;


        // Cast a ray down from above the desired position to find solid ground
        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit groundHitInfo, maxLegLength - minLegLength, walkableLayers)){
            // If solid ground is detected, return the hit point
            groundNormal = groundHitInfo.normal;
            return groundHitInfo.point;
        }
        else{
            // If no ground is detected, fully extend leg
            groundNormal = Vector3.up;
            return rayOrigin + (Vector3.down * (maxLegLength - minLegLength));
        }     
    }


    private IEnumerator MoveLeg(FastIKFabric leg, Vector3 newPosition, Vector3 newNormal){
        // Moves the given leg along a path defined by direction to the new target and the step animation curve

        Vector3 oldPosition = leg.Target.position;
        Vector3 totalDisplacement = newPosition - oldPosition;

        newNormal.Normalize();

        float percent = 0;
        while (percent <= 1){
            percent = Mathf.Clamp01(percent);

            Vector3 currentHeight = stepHeightCurve.Evaluate(percent) * stepAnimationHeight * newNormal;
            Vector3 currentDirection = percent * totalDisplacement;

            Vector3 currentPosition = oldPosition + currentDirection + currentHeight;

            leg.Target.position = currentPosition;

            percent += Time.deltaTime / stepAnimationDuration;
            yield return null;
        }

        leg.Target.position = newPosition;

        yield break;
    }


    private FastIKFabric GetLeg(bool isLeft){
        return isLeft ? leftLegIK : rightLegIK;
    }


    private Quaternion GetLegJointTargetRotation(bool isLeft, bool isLowerLeg){
        // Calculates the target position and rotation of a lower leg rigidbody using two IK bones
        
        FastIKFabric leg = GetLeg(isLeft);

        Vector3 outerBonePosition = isLowerLeg ? leg.transform.position : leg.transform.parent.position;
        Vector3 innerBonePosition = isLowerLeg ? leg.transform.parent.position : leg.transform.parent.parent.position;

        Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, innerBonePosition - outerBonePosition);
        
        return targetRotation;
    }

}
