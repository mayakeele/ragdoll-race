using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveRagdoll : MonoBehaviour
{
    [Header("Component References")]
    public Player player;
    public LegsIKCalculator legsIKCalculator;
    public ArmsActionCoordinator armsActionCoordinator;
    [SerializeField] private Rigidbody pelvisRigidbody;
    [SerializeField] private List<Rigidbody> bodyPartRigidbodies;


    [Header("Enumerated Body Part References")]
    public Transform headTransform;
    [Space]
    public Transform torsoUpperTransform;
    public Transform torsoMiddleTransform;
    public Transform torsoLowerTransform;
    [Space]
    public Transform leftArmInnerTransform;
    public Transform leftArmOuterTransform;
    public Transform rightArmInnerTransform;
    public Transform rightArmOuterTransform;
    [Space]
    public Transform leftLegInnerTransform;
    public Transform leftLegOuterTransform;
    public Transform rightLegInnerTransform;
    public Transform rightLegOuterTransform;



    [Header("Ground Detection Settings")]
    [SerializeField] private LayerMask standableLayers;
    [SerializeField] private float groundedRayLength;
    
    
    [Header("Pelvis Buoyancy Settings")]
    [SerializeField] private float targetPelvisHeight;
    [SerializeField] private float buoyancyMultiplier;
    [SerializeField] private float legsSpringConstant;
    [SerializeField] private float legsSpringDamping;


    [Header("Pelvis Torque Settings")]
    [SerializeField] private float pelvisRotationSpringConstant;
    [SerializeField] private float pelvisRotationDampingConstant;
    [SerializeField] private float pelvisRotationSnapAngle;


    [Header("Leg Settings")]
    [SerializeField] private List<JointTargetFollower> legJointTargetFollowers;
    [Space]
    [SerializeField] private List<Collider> legColliders;
    [Space]
    [SerializeField] private PhysicMaterial legPhysicMaterialRagdoll;
    [SerializeField] private PhysicMaterial legPhysicMaterialWalking;


    [Header("Arm Settings")]
    [SerializeField] private List<JointTargetFollower> armJointTargetFollowers;



    // Variables
    private float bodyMass;
    private List<JointDrive> bodyPartDefaultJointMotors = new List<JointDrive>();
    private bool isPerformingGetup;
    private bool isPerformingJump;
    



    // Unity Functions
    void Awake()
    {
        // Store initial angular joint motor values
        foreach(Rigidbody bodyPart in bodyPartRigidbodies){
            ConfigurableJoint joint = bodyPart.GetComponent<ConfigurableJoint>();
            if(joint){
                // Store joint motor value
                bodyPartDefaultJointMotors.Add(joint.slerpDrive);
            }
            else{
                // Store an empty joint motor to maintain same number of indices
                bodyPartDefaultJointMotors.Add(new JointDrive());
            }
        }
    }


    void Start()
    {
        RecalculateBodyMass();
        //SetJointMotorsState(true);
    }


    void FixedUpdate()
    {   

        // Apply an upward constant force plus an extra spring force on the pelvis if it is near the floor, and is not in ragdoll mode

        if(!isPerformingJump && !player.isRagdoll){

            if(Physics.Raycast(pelvisRigidbody.worldCenterOfMass, Vector3.down, out RaycastHit hitInfo, groundedRayLength, standableLayers)){

                // Update the relative speed of the ground (frame of reference)

                player.isGrounded = true;
                UpdateGroundTrackingVariables(hitInfo);


                // Calculate buoyancy force, as a fraction of the total body mass to provide "neutral buoyancy"
                Vector3 pelvisForce = bodyMass * Physics.gravity.magnitude * buoyancyMultiplier * Vector3.up;

                // Add an extra spring force which increases as legs compress, also provides damping
                float targetHeight = player.groundPosition.y + targetPelvisHeight;
                //float targetHeight = legsIKCalculator.GetFootAnchors().MaxComponents().y + targetPelvisHeight;
                pelvisForce += CalculateUpwardForce(pelvisRigidbody.worldCenterOfMass.y, targetHeight, pelvisRigidbody.velocity.y, bodyMass, legsSpringConstant, legsSpringDamping, false);    
                

                // Apply the total upwards force to the pelvis
                pelvisRigidbody.AddForce(pelvisForce);

                // If the player is standing on an object with a rigidbody, record which object that is and apply an equal and opposite force to it
                if(player.groundRigidbody){
                    player.groundRigidbody.AddForceAtPosition(-pelvisForce, player.groundPosition);
                }


                // Reset air jumps on contact with the ground
                player.controller.ResetAirJumpCount();
                
            }
            else{
                player.isGrounded = false;
            }      
        }
        else{
            player.isGrounded = false;
        }


        // Attempt to correct pelvis rotation by applying torque if not in ragdoll state

        if(!player.isRagdoll){
            Vector3 currentPelvisDirection = pelvisRigidbody.transform.up;
            Vector3 targetPelvisDirection = Vector3.up;
            Vector3 angularVelocity = new Vector3(pelvisRigidbody.angularVelocity.x, 0, pelvisRigidbody.angularVelocity.z);

            Vector3 pelvisTorque = DampedSpring.GetDampedSpringTorque(currentPelvisDirection, targetPelvisDirection, angularVelocity, pelvisRotationSpringConstant, pelvisRotationDampingConstant);
            pelvisRigidbody.AddTorque(pelvisTorque);
        }
        
    }



    // Public Functions
    
    public float GetBodyMass(){
        return bodyMass;
    }


    public void SetJointMotorsState(bool motorsState){
        // Enables or disables all joint motors, (active vs. passive ragdoll)
        for(int i = 0; i < bodyPartRigidbodies.Count; i++){

            Rigidbody bodyPart = bodyPartRigidbodies[i];
            ConfigurableJoint joint = bodyPart.GetComponent<ConfigurableJoint>();

            if(joint){

                // Sets the slerp drive values to their default strength if true, zero if false
                if(motorsState == true){
                    joint.slerpDrive = bodyPartDefaultJointMotors[i];
                }
                else{
                    JointDrive zeroDrive = new JointDrive();
                    zeroDrive.positionSpring = 0;
                    zeroDrive.positionDamper = 0;
                    zeroDrive.maximumForce = 0;

                    joint.slerpDrive = zeroDrive;
                }
            }
        }

        // Tell the arm action coordingator to update its own joint values
        armsActionCoordinator.UpdateArmTorsoJoints();
    }


    public void SetLegPhysicMaterial(bool ragdollState){
        // Changes the leg collider physic materials based on whether the player is in ragdoll state or not
        PhysicMaterial currentMaterial = ragdollState ? legPhysicMaterialRagdoll : legPhysicMaterialWalking;

        foreach(Collider collider in legColliders){
            collider.material = currentMaterial;
        }
    }


    public IEnumerator PerformJump(float speedChange, int numPhysicsFrames, float jumpSpringDisableTime){
        // Sets the 'performing jump' flag true for a bit to prevent the damping spring from stopping jump momentum
        isPerformingJump = true;

        float jumpForce = GetBodyMass() * speedChange / (Time.fixedDeltaTime * numPhysicsFrames);
        
        for(int i = 0; i < numPhysicsFrames; i++){
            pelvisRigidbody.AddForce(jumpForce * Vector3.up);
            yield return new WaitForFixedUpdate();
        }       

        yield return new WaitForSeconds(jumpSpringDisableTime - (numPhysicsFrames * Time.fixedDeltaTime));
        isPerformingJump = false;
    }


    public void MoveToPosition(Vector3 newRootPosition){
        // Teleports the ragdoll's root to a specified position, bringing along the rest of the body and removing all velocity

        // Negate all body part velocities
        foreach(Rigidbody bodyPart in bodyPartRigidbodies){
            bodyPart.velocity = Vector3.zero;
            bodyPart.angularVelocity = Vector3.zero;
        }

        // Move the root to position. All other body parts of children of root and will follow along automatically
        pelvisRigidbody.transform.position = newRootPosition;
    }


    public Vector3 GetRelativeVelocity(){
        // Returns the pelvis velocity relative to the current ground frame of reference

        return pelvisRigidbody.velocity - player.groundVelocity;
    }



    // Private Functions

    private void RecalculateBodyMass(){
        bodyMass = 0;
        foreach(Rigidbody rb in bodyPartRigidbodies){
            bodyMass += rb.mass;
        }
    }


    private Vector3 BodyCenterOfMass(){
        // Calculates the world space center of mass of the ragdoll's body parts
        float totalMass = 0;
        Vector3 centerOfMass = Vector3.zero;

        foreach(Rigidbody rb in bodyPartRigidbodies){
            totalMass += rb.mass;

            centerOfMass += rb.mass * rb.worldCenterOfMass;
        }

        return centerOfMass / totalMass;
    }


    private void UpdateGroundTrackingVariables(RaycastHit hitInfo){
        // Updates the player's varables which track the ground
        player.groundTransform = hitInfo.transform;
        player.groundPosition = hitInfo.point;
        player.groundNormal = hitInfo.normal;

        MovingPlatform movingPlatform = hitInfo.transform.GetComponent<MovingPlatform>();

        if(movingPlatform){
            player.groundRigidbody = movingPlatform.rigidbody;
            player.groundVelocity = movingPlatform.GetPointVelocity(player.groundPosition);
        }
        else if(hitInfo.rigidbody){
            player.groundRigidbody = hitInfo.rigidbody;
            player.groundVelocity = player.groundRigidbody.GetPointVelocity(player.groundPosition);

        }
        else{
            player.groundRigidbody = null;
            player.groundVelocity = Vector3.zero;
        }
    }


    private Vector3 CalculateUpwardForce(float currentHeight, float targetHeight, float verticalSpeed, float liftedMass, float springConstant, float dampingConstant, bool ignoreAboveTarget){
        // Calculates the upwards force of a virtual spring, like a marionette doll

        Vector3 upwardForce;

        if(ignoreAboveTarget && currentHeight > targetHeight){
            // Legs are fully extended, no force
            upwardForce = Vector3.zero;
        }
        else{
            // Legs are partially compressed, acting as a damped spring
            Vector3 relativePosition = new Vector3(0, currentHeight - targetHeight, 0);
            Vector3 relativeVelocity = new Vector3(0, verticalSpeed, 0);
            upwardForce = DampedSpring.GetDampedSpringForce(relativePosition, relativeVelocity, liftedMass, springConstant, dampingConstant);
        }

        return upwardForce;
    }

}
