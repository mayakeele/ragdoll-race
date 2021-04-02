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
    [SerializeField] private AnimationCurve stepDistanceCurve;
    [SerializeField] private AnimationCurve stepHeightCurve;


    [Header("Step Calculation Properties")]
    [SerializeField] private float standingFeetWidth;
    [SerializeField] private float minLegExtension;
    [SerializeField] private float maxLegExtension;
    [SerializeField] private LayerMask walkableLayers;
    [SerializeField] private bool boneTransformOnJoint;
    

    
    // Private Variables

    private Quaternion leftUpperRotLocal;
    private Quaternion rightUpperRotLocal;
    private Quaternion leftLowerRotLocal;
    private Quaternion rightLowerRotLocal;
    private Quaternion leftUpperRotGlobal;
    private Quaternion rightUpperRotGlobal;
    private Quaternion leftLowerRotGlobal;
    private Quaternion rightLowerRotGlobal;

    private bool useDynamicGait;
    private float timeSinceLastStep;
    private bool leftLegMoving;

    private Vector3 leftFootAnchor;
    private Vector3 rightFootAnchor;



    // Unity Functions
    
    void Start()
    {
        leftLowerRotLocal = leftLowerJoint.transform.localRotation;
        leftUpperRotLocal = leftUpperJoint.transform.localRotation;
        rightLowerRotLocal = rightUpperJoint.transform.localRotation;
        rightUpperRotLocal = rightUpperJoint.transform.localRotation;

        leftLowerRotGlobal = leftLowerJoint.transform.rotation;
        leftUpperRotGlobal = leftUpperJoint.transform.rotation;
        rightLowerRotGlobal = rightUpperJoint.transform.rotation;
        rightUpperRotGlobal = rightUpperJoint.transform.rotation;
    }


    void FixedUpdate()
    {
        timeSinceLastStep += Time.fixedDeltaTime;


        // Update leg movement variables based on the player's speed, using empirically-derived curves

        float speed = pelvisRigidbody.velocity.magnitude;
        float speedSquared = Mathf.Pow(speed, 2);
        float speedCubed = Mathf.Pow(speed, 3);

        stepCycleLength = 1.86f - (0.912f * speed) + (0.269f * speedSquared) - (0.0303f * speedCubed);
        movingStepOffset = 1.13f - (0.546f * speed) + (0.208f * speedSquared) - (0.0282f * speedCubed);
        stepAnimationHeight = 0.0788f + (0.0459f * speed);


        // Only articulate legs if the player is grounded and not ragdolled
        if(activeRagdoll.player.isGrounded && !activeRagdoll.player.isRagdoll){

            // Move the current leg's IK bone if enough time has passed

            if(timeSinceLastStep >= stepCycleLength / 2){
                timeSinceLastStep = 0;

                FastIKFabric currentLeg = leftLegMoving ? leftLegIK : rightLegIK;
                Transform currentTarget = currentLeg.Target;

                Vector3 desiredPosition = CastToGround(leftLegMoving, out Vector3 groundNormal);
                float displacementFromDefault = Vector3.Distance(currentTarget.position, desiredPosition);

                if(displacementFromDefault >= minDisplacementToMove){
                    StartCoroutine(MoveLeg(currentLeg, desiredPosition, groundNormal));
                    if(leftLegMoving){ leftFootAnchor = desiredPosition; }  else{ rightFootAnchor = desiredPosition; }
                }

                leftLegMoving = !leftLegMoving;
            }

            
            // Update position and rotation targets for the physical legs

            Quaternion localRotation;

            localRotation = Quaternion.Inverse(leftLowerJoint.transform.parent.rotation) * GetLegJointTargetRotation(true, true , leftLowerRotGlobal);
            leftLowerJoint.SetTargetRotationLocal(localRotation, leftLowerRotLocal);

            localRotation = Quaternion.Inverse(leftUpperJoint.transform.parent.rotation) * GetLegJointTargetRotation(true, false, leftUpperRotGlobal);
            leftUpperJoint.SetTargetRotationLocal(localRotation, leftUpperRotLocal);

            localRotation = Quaternion.Inverse(rightLowerJoint.transform.parent.rotation) * GetLegJointTargetRotation(false, true, rightLowerRotGlobal);
            rightLowerJoint.SetTargetRotationLocal(localRotation, rightLowerRotLocal);

            localRotation = Quaternion.Inverse(rightUpperJoint.transform.parent.rotation) * GetLegJointTargetRotation(false, false, rightUpperRotGlobal);
            rightUpperJoint.SetTargetRotationLocal(localRotation, rightUpperRotLocal);

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

    private Vector3 CastToGround(bool isLeft, out Vector3 groundNormal){
        // Casts a ray down and through the desired position to find solid ground

        // Calculate the horizontal and max possible vertical position of the foot
        Vector3 rayOrigin = pelvisRigidbody.worldCenterOfMass;
        rayOrigin += (activeRagdoll.player.rootForward.right.ProjectHorizontal() * (standingFeetWidth/2) * (isLeft ? -1 : 1));
        rayOrigin += pelvisRigidbody.velocity.ProjectHorizontal() * movingStepOffset;
        //rayOrigin += Vector3.down * minLegExtension;


        // Cast a ray down from above the desired position to find solid ground
        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit groundHitInfo, maxLegExtension, walkableLayers)){

            // If solid ground is detected between the maximum and minimum leg extension, return the hit point
            if(groundHitInfo.distance > minLegExtension){
                groundNormal = groundHitInfo.normal;
                return groundHitInfo.point;
            }
            // If it is above the minimum leg extension, return the height of minimum leg extension
            else{
                groundNormal = groundHitInfo.normal;
                return rayOrigin + (Vector3.down * minLegExtension);
            }
            
        }
        else{
            // If no ground is detected, fully extend leg
            groundNormal = Vector3.up;
            return rayOrigin + (Vector3.down * maxLegExtension);
        }     
    }


    private IEnumerator MoveLeg(FastIKFabric leg, Vector3 newPosition, Vector3 newNormal){
        // Moves the given leg along a path defined by direction to the new target and the step animation curve

        Vector3 oldPosition = leg.Target.position;
        Vector3 totalDisplacement = newPosition - oldPosition;

        newNormal.Normalize();

        float timeGradient = 0;
        while (timeGradient <= 1){
            timeGradient = Mathf.Clamp01(timeGradient);

            float spaceGradient = stepDistanceCurve.Evaluate(timeGradient);

            Vector3 currentHeight = stepHeightCurve.Evaluate(spaceGradient) * stepAnimationHeight * newNormal;
            Vector3 currentDirection = spaceGradient * totalDisplacement;

            Vector3 currentPosition = oldPosition + currentDirection + currentHeight;
            leg.Target.position = currentPosition;

            timeGradient += Time.fixedDeltaTime / stepAnimationDuration;

            yield return new WaitForFixedUpdate();
        }

        leg.Target.position = newPosition;

        yield break;
    }


    private FastIKFabric GetLeg(bool isLeft){
        return isLeft ? leftLegIK : rightLegIK;
    }


    private Quaternion GetLegJointTargetRotation(bool isLeft, bool isLowerLeg, Quaternion initialRotation){
        // Calculates the target position and rotation of a leg segment rigidbody using two IK bones
        
        FastIKFabric leg = GetLeg(isLeft);

        Vector3 outerBonePosition = isLowerLeg ? leg.transform.position : leg.transform.parent.position;
        Vector3 innerBonePosition = isLowerLeg ? leg.transform.parent.position : leg.transform.parent.parent.position;

        Vector3 jointPosition = boneTransformOnJoint ? innerBonePosition : (outerBonePosition + innerBonePosition) / 2;
        Vector3 jointDirectionHorizontal = leg.Pole.position.ProjectHorizontal() - jointPosition.ProjectHorizontal();

        Quaternion yawRotation = Quaternion.FromToRotation(Vector3.forward, jointDirectionHorizontal);
        Quaternion pitchRotation = Quaternion.FromToRotation(Vector3.down, outerBonePosition - innerBonePosition);

        Quaternion worldRotation = pitchRotation * yawRotation;
        return worldRotation * initialRotation;
    }

}
