using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmsActionCoordinator : MonoBehaviour
{

    [Header("References")]

    public ActiveRagdoll activeRagdoll;
    [SerializeField] private List<ConfigurableJoint> armJoints;
    [Space]
    [SerializeField] private List<ConfigurableJoint> lowerTorsoJoints;
    [SerializeField] private List<ConfigurableJoint> upperTorsoJoints;


    [Header("Arm Strength Properties")]
    
    [SerializeField] private float armStrengthLow;
    [SerializeField] private float armDampingLow;
    [Space]
    [SerializeField] private float armStrengthHigh;
    [SerializeField] private float armDampingHigh;


    [Header("Lower Torso Strength Properties")]
    [SerializeField] private float lowerTorsoStrengthLow;
    [SerializeField] private float lowerTorsoDampingLow;
    [Space]
    [SerializeField] private float lowerTorsoStrengthHigh;
    [SerializeField] private float lowerTorsoDampingHigh;
    

    [Header("Upper Torso Strength Properties")]
    [SerializeField] private float upperTorsoStrengthLow;
    [SerializeField] private float upperTorsoDampingLow;
    [Space]
    [SerializeField] private float upperTorsoStrengthHigh;
    [SerializeField] private float upperTorsoDampingHigh;



    // Private Variables
    bool isActionButtonHeld = false;



    // Unity Functions
    void Start()
    {
        // Initialize arms and body in relaxed state
        SetArmJointsState(false);
        SetTorsoJointsState(false);
    }


    // Public Functions

    public void OnArmActionButtonPressed(){
        // Stiffen the arm and torso joints when the action button is first held

        isActionButtonHeld = true;
        UpdateArmTorsoJoints();
    }

    public void OnArmActionButtonReleased(){
        // Unstiffen the arm and torso joints when the action button is released

        isActionButtonHeld = false;
        UpdateArmTorsoJoints();
    }



    public void UpdateArmTorsoJoints(){
        // Updates the sprigng and damping values for the arms and torso based on whether the action button is being held
        SetArmJointsState(isActionButtonHeld);
        SetTorsoJointsState(isActionButtonHeld);
    }




    // Private Functions

    private void SetArmJointsState(bool setHigh){
        // Sets the joint drive properties on each arm either high or low

        foreach(ConfigurableJoint joint in armJoints){
            if(setHigh) joint.SetSlerpDrive(armStrengthHigh, armDampingHigh);
            else joint.SetSlerpDrive(armStrengthLow, armDampingLow);
        }
    }


    private void SetTorsoJointsState(bool setHigh){
        // Sets the joint drive properties on the upper and lower torso joints either high or low

        foreach(ConfigurableJoint joint in lowerTorsoJoints){
            if(setHigh) joint.SetSlerpDrive(lowerTorsoStrengthHigh, lowerTorsoDampingHigh);
            else joint.SetSlerpDrive(lowerTorsoStrengthLow, lowerTorsoDampingLow);
        }

        foreach(ConfigurableJoint joint in upperTorsoJoints){
            if(setHigh) joint.SetSlerpDrive(upperTorsoStrengthHigh, upperTorsoDampingHigh);
            else joint.SetSlerpDrive(upperTorsoStrengthLow, upperTorsoDampingLow);
        }
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

}
