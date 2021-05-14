using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmsActionCoordinator : MonoBehaviour
{

    [Header("References")]

    public ActiveRagdoll activeRagdoll;
    [SerializeField] private List<ConfigurableJoint> armJoints;


    [Header("Arm Strength Properties")]
    
    [SerializeField] private float armStrengthLow;
    [SerializeField] private float armDampingLow;
    [Space]
    [SerializeField] private float armStrengthHigh;
    [SerializeField] private float armDampingHigh;
    


    // Private Variables


    // Unity Functions
    void Start()
    {
        foreach(ConfigurableJoint joint in armJoints){
            joint.SetSlerpDrive(armStrengthLow, armDampingLow);
        }
    }


    // Public Functions

    public void OnArmActionButtonPressed(){
        // Set both arms' joint muscle strenth to high when the button is held, low when released

        foreach(ConfigurableJoint joint in armJoints){
            joint.SetSlerpDrive(armStrengthHigh, armDampingHigh);
        }
    }


    public void OnArmActionButtonReleased(){

        foreach(ConfigurableJoint joint in armJoints){
            joint.SetSlerpDrive(armStrengthLow, armDampingLow);
        }
    }



    // Private Functions

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
