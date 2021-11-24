using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DitzelGames.FastIK;

public class Arm : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private ArmsActionCoordinator armsActionCoordinator;
    private Transform pelvisTransform;
    [Space]
    public Transform armRootTransform;
    public Transform armEndTransform;
    [HideInInspector] public FastIKFabric armIK;
    public List<ConfigurableJoint> joints;


    [Header("Idle Properties")]
    [SerializeField] private Vector3 idlePositionRelativeToPelvis;


    [Header("Punch Properties")]
    [SerializeField] private float armLength;
    [SerializeField] private float punchSpeed;
    [SerializeField] private float punchRecoveryDuration;
    [SerializeField] private AnimationCurve punchCurveForward;
    [SerializeField] private AnimationCurve punchCurveVertical;
    [SerializeField] private AnimationCurve punchCurveSideways;


    [Header("Climbing Properties")]
    [SerializeField] private LayerMask climbableLayers;


    [Header("Action States")]
    public bool isActing = false;
    public bool isHoldingObject = false;

    private float actionTimer = 0;



    // Unity Functions
    void Awake()
    {
        armIK = armEndTransform.GetComponent<FastIKFabric>();
        pelvisTransform = armsActionCoordinator.activeRagdoll.player.rootForward;
    }

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        // Update action timer
        if(actionTimer > 0){
            actionTimer -= Time.fixedDeltaTime;
            isActing = true;
        }
        else{
            actionTimer = 0;
            isActing = false;
        }


        // If no actions are taking place, move this arm to its idle position
        if(!isActing){
            //SetIKTargetPosition(pelvisTransform.TransformPoint(idlePositionRelativeToPelvis));
        }
    }



    // Public Functions

    public void SetIKTargetPosition(Vector3 targetPostion){
        // Sets the IK chain's world space target position on the specified arm
        armIK.Target.position = targetPostion;
    }


    public bool Punch(Vector3 worldTarget){
        // Continually updates arm IK target over a period of time, using curves relative to the initial arm position

        if(!isActing){
            StartCoroutine(PunchCoroutine(worldTarget));
        }

        return !isActing;
    }

    public IEnumerator PunchCoroutine(Vector3 finalTargetWorld){
        // Continually updates arm IK target over a period of time, using curves relative to the initial arm position

        Transform pelvisTransform = armsActionCoordinator.activeRagdoll.player.rootForward;

        Vector3 initialTargetWorld = armEndTransform.position;

        Vector3 initialTargetLocal = pelvisTransform.InverseTransformPoint(initialTargetWorld);
        Vector3 finalTargetLocal = pelvisTransform.InverseTransformPoint(finalTargetWorld);
        Vector3 totalDisplacementLocal = finalTargetLocal - initialTargetLocal;

        float punchDuration = totalDisplacementLocal.magnitude / punchSpeed;
        TrySetActionTimer(punchDuration);


        // Move target locally along curves over time, and calculate corresponding world space IK target
        float elapsedTime = 0;
        while(elapsedTime < punchDuration){
            elapsedTime += Time.fixedDeltaTime;

            float timeGradient = elapsedTime.GetGradientClamped(0, punchDuration);

            Vector3 currentDisplacementLocal = new Vector3(
                totalDisplacementLocal.x * punchCurveSideways.Evaluate(timeGradient),
                totalDisplacementLocal.y * punchCurveVertical.Evaluate(timeGradient),
                totalDisplacementLocal.z * punchCurveForward.Evaluate(timeGradient)
            );

            Vector3 currentPositionWorld = pelvisTransform.TransformPoint(initialTargetLocal + currentDisplacementLocal);
            SetIKTargetPosition(currentPositionWorld);

            yield return new WaitForFixedUpdate();
        }

        TrySetActionTimer(punchRecoveryDuration);
    }



    // Private Functions

    private bool TrySetActionTimer(float timerDuration){
        // Sets the action timer to a certain time only if an action is not currently being performed
        // Returns whether the time was set successfully

        if(isActing){
            return false;
        }
        else{
            actionTimer = timerDuration;
            return true;
        }
    }
}
