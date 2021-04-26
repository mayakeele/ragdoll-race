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


    
    [Header("Action Criteria")]
    [SerializeField] private float punchDetectionRange;



    // Private Variables



    // Unity Functions

    void Awake()
    {

    }

    void Start()
    {
        
    }


    void FixedUpdate()
    {
        // Update timers
    }



    // Public Functions

    public void OnLActionButtonPressed(){
        ArmAction chosenAction = ChooseArmAction(true, out Vector3 targetPosition);
        TriggerArmAction(true, chosenAction, targetPosition);
    }

    public void OnRActionButtonPressed(){
        ArmAction chosenAction = ChooseArmAction(false, out Vector3 targetPosition);
        TriggerArmAction(false, chosenAction, targetPosition);
    }

    public void OnLActionButtonReleased(){

    }
    public void OnRActionButtonReleased(){

    }



    // Private Functions

    private void TriggerArmAction(bool isLeft, ArmAction chosenAction, Vector3 targetPosition){
        // Perform the action chosen for this arm
        Arm thisArm = isLeft ? leftArm : rightArm;

        switch(chosenAction){
            case ArmAction.Idle:

            break;

            case ArmAction.Punch:
                thisArm.Punch(targetPosition);
            break;

            case ArmAction.GrabWeapon:

            break;

            case ArmAction.SwingWeapon:

            break;

            case ArmAction.DropWeapon:

            break;

            case ArmAction.Climb:

            break;
        }
    }

    private ArmAction ChooseArmAction(bool isLeft, out Vector3 targetPosition){
        // Decides from context which action to pursue when a button is pressed
        targetPosition = Vector3.zero;
        return ArmAction.Punch;
    }


    private void SetArmPositionToIdle(bool isLeft){
        // Sets the given arm's IK target to its default location, relative to its root

    }


    


    private Player FindClosestPlayer(){
        // Searches through all players and returns the one with the closest root

        float minDist = 1000000;
        Player closestPlayer = null;

        foreach(Player other in activeRagdoll.player.manager.GetAllPlayers()){
            if(other != activeRagdoll.player){
                float currDist = Vector3.Distance(activeRagdoll.player.rootRigidbody.position, other.rootRigidbody.position);
                if(currDist < minDist){
                    minDist = currDist;
                    closestPlayer = other;
                }
            }   
        }
        
        return closestPlayer;
    }
}
