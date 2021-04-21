using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointTargetFollower : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform targetTransform;


    [Header("Follow Properties")]

    [Space]
    [SerializeField] private bool boneTransformOnJoint;



    // Private Variables
    private ConfigurableJoint joint;
    private Quaternion initialRotationLocal;
    private Quaternion initialRotationGlobal;
    private float defaultJointSpring;
    private float defaultJointDamping;

    private bool isFollowingTarget;



    // Unity Functions
    void Start()
    {
        joint = GetComponent<ConfigurableJoint>();

        initialRotationLocal = joint.transform.localRotation;
        initialRotationGlobal = joint.transform.rotation;

        defaultJointSpring = joint.slerpDrive.positionSpring;
        defaultJointDamping = joint.slerpDrive.positionDamper;

        isFollowingTarget = true;
    }


    void Update()
    {
        // Move joint towards its target if followTarget flag is true
        if(isFollowingTarget){
            joint.SetTargetRotationLocal(GetJointTargetRotation(), initialRotationLocal);
        }
    }



    // Public Functions

    public void SetTargetFollowState(bool followState){

        isFollowingTarget = followState;

        JointDrive drive = new JointDrive();
        drive.positionSpring = followState ? defaultJointSpring : 0;
        drive.positionDamper = followState ? defaultJointDamping : 0;

        joint.slerpDrive = drive;
    }



    // Private Functions
    private Quaternion GetJointTargetRotation(){
        // Calculates the target rotation of the joint relative to its parent and its initial rotation

        Quaternion localRotation = Quaternion.Inverse(joint.transform.parent.rotation) * targetTransform.rotation * initialRotationGlobal;

        return localRotation;
    }
}
