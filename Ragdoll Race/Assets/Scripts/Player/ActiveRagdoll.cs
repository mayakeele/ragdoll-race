using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveRagdoll : MonoBehaviour
{
    [Header("Component References")]
    public Player player;
    public LegManager legManager;
    [SerializeField] private Rigidbody pelvisRigidbody;
    [SerializeField] private Rigidbody lowerTorsoRigidbody;
    [SerializeField] private Rigidbody upperTorsoRigidbody;
    [SerializeField] private Rigidbody headRigidbody;
    [SerializeField] private List<Rigidbody> bodyPartRigidbodies;    


    [Header("Detection Settings")]
    [SerializeField] private LayerMask walkableLayers;


    
    [Header("Legs Settings")]
    [SerializeField] private float targetLegsLength;
    [SerializeField] private float legsSpringConstant;
    [SerializeField] private float legsSpringDamping;


    [Header("Pelvis Settings")]
    [SerializeField] private float pelvisRotationSpringConstant;
    [SerializeField] private float pelvisRotationDampingConstant;
    [SerializeField] private float pelvisRotationSnapAngle;



    // Private Variables
    private float bodyMass;
    private bool isPerformingGetup;
    private bool isPerformingJump;



    // Unity Functions
    void Awake()
    {
        
    }


    void Start()
    {
        RecalculateBodyMass();
        //SetJointMotorsState(true);
    }


    void FixedUpdate()
    {   

        // Apply an upward spring force on the pelvis if it is near the floor, and is not in ragdoll mode

        if(!isPerformingJump && !player.isRagdoll && Physics.Raycast(pelvisRigidbody.worldCenterOfMass, Vector3.down, out RaycastHit hitInfo, targetLegsLength, walkableLayers)){

            //float targetHeight = hitInfo.point.y + targetLegsLength;
            float targetHeight = legManager.GetFootAnchors().MaxComponents().y + targetLegsLength;
            Vector3 pelvisForce = CalculateUpwardForce(pelvisRigidbody.worldCenterOfMass.y, targetHeight, pelvisRigidbody.velocity.y, bodyMass, legsSpringConstant, legsSpringDamping);    
            
            pelvisRigidbody.AddForce(pelvisForce);

            player.isGrounded = true;
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


    public void SetPelvisRotationConstraint(bool constrainY = false){
        // Constrains pelvis rotation to either the Y axis only, or no rotation at all
        if(constrainY){
            pelvisRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
        }
        else{
            pelvisRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
    }

    public void RemovePelvisRotationConstraints(){
        // Removes all constraints on the pelvis rigidbody's rotation
        pelvisRigidbody.constraints = RigidbodyConstraints.None;
    }


    public void SetJointMotorsState(bool motorsState){
        // Enables or disables all joint motors, (active vs. passive ragdoll)
        foreach(Rigidbody bodyPart in bodyPartRigidbodies){
            ConfigurableJoint joint = bodyPart.GetComponent<ConfigurableJoint>();
            if(joint){
                // Sets the angular drive mode to X and YZ to activate, or Slerp (which is unused) to passivate
                joint.rotationDriveMode = motorsState ? RotationDriveMode.XYAndZ : RotationDriveMode.Slerp;
            }
        }

        // Updates pelvis rotation constraints
        if(motorsState == false){
            //RemovePelvisRotationConstraints();
        }
        else{
            if(!isPerformingGetup){
                //StartCoroutine(ResetPelvisRotation(pelvisRotationSpringConstant, pelvisRotationDampingConstant, pelvisRotationSnapAngle));
            }  
        }
    }


    public IEnumerator PerformJump(float finalSpeed, int numPhysicsFrames, float jumpSpringDisableTime){
        // Sets the 'performing jump' flag true for a bit to prevent the damping spring from stopping jump momentum
        isPerformingJump = true;

        float jumpForce = GetBodyMass() * finalSpeed / (Time.fixedDeltaTime * numPhysicsFrames);
        
        for(int i = 0; i < numPhysicsFrames; i++){
            pelvisRigidbody.AddForce(jumpForce * Vector3.up);
            yield return new WaitForFixedUpdate();
        }       

        yield return new WaitForSeconds(jumpSpringDisableTime - (numPhysicsFrames * Time.fixedDeltaTime));
        isPerformingJump = false;
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


    private Vector3 CalculateUpwardForce(float currentHeight, float targetHeight, float verticalSpeed, float liftedMass, float springConstant, float dampingConstant){
        // Calculates the upwards force of a virtual spring, like a marionette doll

        Vector3 upwardForce;

        if(currentHeight > targetHeight){
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


    private IEnumerator ResetPelvisRotation(float springConstant, float springDamping, float snapAngleThreshold){
        // Applies a torque on the hips towards an upright orientation, with its y-axis facing upwards
        // Once within a certain angular distance, snaps to target and locks in place

        isPerformingGetup = true;

        Vector3 currentPelvisDirection = pelvisRigidbody.transform.up;
        Vector3 targetPelvisDirection = Vector3.up;
   
        float currentAngle = Vector3.Angle(currentPelvisDirection, targetPelvisDirection);

        while(currentAngle > snapAngleThreshold){
            // Apply torque towards the target
            Vector3 pelvisTorque = DampedSpring.GetDampedSpringTorque(currentPelvisDirection, targetPelvisDirection, pelvisRigidbody.angularVelocity, springConstant, springDamping);
            pelvisRigidbody.AddTorque(pelvisTorque);

            // Update rotation tracking variables
            currentPelvisDirection = pelvisRigidbody.transform.up;
            currentAngle = Vector3.Angle(currentPelvisDirection, targetPelvisDirection);

            yield return new WaitForFixedUpdate();
        }

        // Snap to target once the pelvis is close enough to upright and lock it in place
        Quaternion snapRotation = Quaternion.LookRotation(pelvisRigidbody.transform.forward.ProjectHorizontal(), Vector3.up);
        pelvisRigidbody.MoveRotation(snapRotation);

        SetPelvisRotationConstraint();

        isPerformingGetup = false;
    }
}
