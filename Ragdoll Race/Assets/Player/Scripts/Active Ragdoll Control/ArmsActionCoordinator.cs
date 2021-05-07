using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmsActionCoordinator : MonoBehaviour
{
    private enum ArmAction {
        Idle,
        Punch,
        GrabWeapon,
        SwingWeapon,
        DropWeapon,
        Climb
    }



    [Header("References")]
    public ActiveRagdoll activeRagdoll;
    public Arm leftArm;
    public Arm rightArm;
    [HideInInspector] public List<Arm> arms;


    
    [Header("Action Criteria")]
    [SerializeField] private float punchDetectionRange;
    [SerializeField] private float punchDetectionAngularRange;



    // Private Variables
    private ArmAction currentArmAction;


    // Unity Functions

    void Awake()
    {
        arms = new List<Arm>{leftArm, rightArm};
    }

    void Start()
    {
        
    }


    void FixedUpdate()
    {
        // Update timers
    }



    // Public Functions

    public void OnArmActionButtonPressed(){
        // Choose which action to take, then begin executing it

        // If an enemy is within range, try to punch them with the closer arm
        Player closestPlayer = FindClosestPlayerInRange(punchDetectionRange, punchDetectionAngularRange);
        if(closestPlayer && CanPunch(closestPlayer, out Vector3 punchTarget, out Arm punchingArm)){
            punchingArm.Punch(punchTarget);
        }

        // If a weapon is nearby, try to reach for it
    }


    public void OnArmActionButtonReleased(){
        
    }



    // Private Functions

    private void SetArmPositionToIdle(Arm arm){
        // Sets the given arm's IK target to its default location, relative to its root

    }


    private void SetArmsStrength(float strength){

    }


    private Player FindClosestPlayerInRange(float range, float totalAngularRange){
        // Searches through all players and returns the one with the closest root that is also in angular range of this player's forward direction
        // If no players are within angular range, return null

        float closestDist = range;
        Player closestPlayer = null;

        foreach(Player otherPlayer in activeRagdoll.player.manager.GetAllPlayers()){
            if(otherPlayer != activeRagdoll.player){
                Vector3 displacementToOther = otherPlayer.rootForward.position - activeRagdoll.player.rootForward.position;
                float currDist = displacementToOther.magnitude;
                float currAngle = Vector3.Angle(activeRagdoll.player.rootForward.forward.ProjectHorizontal(), displacementToOther.ProjectHorizontal());

                if((currDist <= closestDist) && (currAngle <= totalAngularRange/2)){
                    closestDist = currDist;
                    closestPlayer = otherPlayer;
                }
            }   
        }
        
        return closestPlayer;
    }


    private bool CanPunch(Player targetPlayer, out Vector3 closestTarget, out Arm closestArm){
        
        List<Vector3> hittableTargets = new List<Vector3>{targetPlayer.activeRagdoll.headTransform.position, targetPlayer.activeRagdoll.torsoTransform.position};
        
        float closestDist = float.MaxValue;
        closestArm = null;
        closestTarget = Vector3.zero;

        foreach(Arm currArm in arms){
            foreach(Vector3 currTarget in hittableTargets){
                float currDist = Vector3.Distance(currArm.armRootTransform.position, currTarget);
                if(!currArm.isActing && currDist < closestDist){
                    closestDist = currDist;
                    closestTarget = currTarget;
                    closestArm = currArm;
                }
            }
        }

        if(closestArm){
            return true;
        }
        else{
            return false;
        }
    }
}
