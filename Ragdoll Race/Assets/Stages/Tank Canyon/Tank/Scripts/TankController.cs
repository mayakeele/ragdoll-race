using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : MonoBehaviour
{
    private enum ProgressionPhase {
        Idle,
        Bridge,
        Desert,
        Pursuit,
        Canyon,
        Boulders,
        Cliff
    }


    [Header("References")]
    [SerializeField] private Rigidbody environmentRigidbody;
    [Space]
    [SerializeField] private CameraController cameraController;
    [Space]
    [SerializeField] private TankAudioManager tankAudioManager;
    [SerializeField] private TreadsManager treadsManager;
    [SerializeField] private TurretManager turretManager;
    [Space]
    [SerializeField] private List<GameObject> knockoutTriggerGroups;


    [Header("Phase Movement Properties")]
    [Space]

    [Header("0 - Idle")]
    [SerializeField] private CameraParametersContainer idleCameraParameters;
    [SerializeField] private float idleOffTime;
    [SerializeField] private float idleOnTime;
    
    [Header("1 - Bridge")]
    [SerializeField] private CameraParametersContainer bridgeCameraParameters;
    [SerializeField] private CameraTransitionParameters bridgeCameraTransitionParameters;
    [SerializeField] private float bridgeSpeed;
    [SerializeField] private float bridgeAccelerationTime;
    [SerializeField] private float bridgeFinalDistance;

    [Header("2 - Desert")]
    [SerializeField] private CameraParametersContainer desertCameraParameters;
    [SerializeField] private CameraTransitionParameters desertCameraTransitionParameters;
    [SerializeField] private float desertSpeed;
    [SerializeField] private float desertAccelerationTime;
    [SerializeField] private float desertFinalDistance;

    [Header("3 - Pursuit")]
    [SerializeField] private CameraParametersContainer pursuitCameraParameters;
    [SerializeField] private CameraTransitionParameters pursuitCameraTransitionParameters;
    [SerializeField] private float pursuitSpeed;
    [SerializeField] private float pursuitAccelerationTime;
    [SerializeField] private float pursuitFinalDistance;

    [Header("4 - Canyon")]
    [SerializeField] private CameraParametersContainer canyonCameraParameters;
    [SerializeField] private CameraTransitionParameters canyonCameraTransitionParameters;
    [SerializeField] private float canyonSpeed;
    [SerializeField] private float canyonAccelerationTime;
    [SerializeField] private float canyonFinalDistance;

    [Header("5 - Boulders")]
    [SerializeField] private CameraParametersContainer bouldersCameraParameters;
    [SerializeField] private CameraTransitionParameters bouldersCameraTransitionParameters;
    [SerializeField] private float bouldersSpeed;
    [SerializeField] private float bouldersAccelerationTime;
    [SerializeField] private float bouldersFinalDistance;

    [Header("6 - Cliff")]
    [SerializeField] private CameraParametersContainer cliffCameraParameters;
    [SerializeField] private CameraTransitionParameters cliffCameraTransitionParameters;
    [SerializeField] private float cliffSpeed;
    [SerializeField] private float cliffAccelerationTime;
    [SerializeField] private float cliffFinalDistance;



    // Current State
    public float tankSpeed;
    private float distanceCovered = 0;
    private ProgressionPhase currentPhase = ProgressionPhase.Idle;



    void Start()
    {
        StartCoroutine(BeginLevel());
    }

    void FixedUpdate()
    {
        if(tankSpeed > 0){
            MoveEnvironment(tankSpeed, Time.fixedDeltaTime);
        }

        CheckIfNextPhaseStarts();
    }



    private void SetTankSpeed(float speed){
        tankSpeed = speed;
        treadsManager.SetTreadSpeed(speed);      
    }


    private void MoveEnvironment(float speed, float timeDelta){
        // Since the tank uses a kinematic rigidbody, position is updated here
        float movementAmount = speed * timeDelta;
        Vector3 movementVector = Vector3.right * movementAmount;

        distanceCovered += movementAmount;

        environmentRigidbody.MovePosition(environmentRigidbody.position + movementVector);
    }


    private void CheckIfNextPhaseStarts(){
        // Checks whether the tank has crossed into the next phase's threshold given the current phase
        // Sets new tank behavior if a new phase is entered
        
        switch(currentPhase){

            case ProgressionPhase.Bridge:
                if(distanceCovered > bridgeFinalDistance){
                    currentPhase = ProgressionPhase.Desert;

                    StartCoroutine(AccelerateTank(desertSpeed, desertAccelerationTime));

                    cameraController.SetParameters(desertCameraParameters, desertCameraTransitionParameters);

                    knockoutTriggerGroups.ActivateOneDeactivateOthers((int)ProgressionPhase.Desert);
                }
            break;

            case ProgressionPhase.Desert:
                if(distanceCovered > desertFinalDistance){
                    currentPhase = ProgressionPhase.Pursuit;

                    StartCoroutine(AccelerateTank(pursuitSpeed, pursuitAccelerationTime));

                    cameraController.SetParameters(pursuitCameraParameters, pursuitCameraTransitionParameters);

                    knockoutTriggerGroups.ActivateOneDeactivateOthers((int)ProgressionPhase.Pursuit);
                }
            break;

            case ProgressionPhase.Pursuit:
                if(distanceCovered > pursuitFinalDistance){
                    currentPhase = ProgressionPhase.Canyon;

                    StartCoroutine(AccelerateTank(canyonSpeed, canyonAccelerationTime));

                    cameraController.SetParameters(canyonCameraParameters, canyonCameraTransitionParameters);

                    knockoutTriggerGroups.ActivateOneDeactivateOthers((int)ProgressionPhase.Canyon);
                }
            break;

            case ProgressionPhase.Canyon:
                if(distanceCovered > canyonFinalDistance){
                    currentPhase = ProgressionPhase.Boulders;

                    StartCoroutine(AccelerateTank(bouldersSpeed, bouldersAccelerationTime));

                    cameraController.SetParameters(bouldersCameraParameters, bouldersCameraTransitionParameters);

                    knockoutTriggerGroups.ActivateOneDeactivateOthers((int)ProgressionPhase.Boulders);
                }
            break;

            case ProgressionPhase.Boulders:
                if(distanceCovered > bouldersFinalDistance){
                    currentPhase = ProgressionPhase.Cliff;

                    StartCoroutine(AccelerateTank(cliffSpeed, cliffAccelerationTime));

                    cameraController.SetParameters(cliffCameraParameters, cliffCameraTransitionParameters);

                    knockoutTriggerGroups.ActivateOneDeactivateOthers((int)ProgressionPhase.Cliff);
                }
            break;

            case ProgressionPhase.Cliff:
                if(distanceCovered > cliffFinalDistance){
                    // End game, freeze controls and let tank fall off cliff
                }
            break;
        }

    }


    private IEnumerator AccelerateTank(float finalSpeed, float timePeriod){
        // Accelerates the tank from its current speed to the given speed, over a given period of time

        float initialSpeed = tankSpeed;

        float currTime = 0;
        while(currTime < timePeriod){
            SetTankSpeed(currTime.Map(0, timePeriod, initialSpeed, finalSpeed));

            currTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        tankSpeed = finalSpeed;
    }



    private IEnumerator BeginLevel(){
        // Turns on the engine after a little while. Then waits a little longer, then starts moving and triggers the next phase

        currentPhase = ProgressionPhase.Idle;
        cameraController.SetParameters(idleCameraParameters);
        SetTankSpeed(0);
        knockoutTriggerGroups.ActivateOneDeactivateOthers((int)ProgressionPhase.Idle);


        tankAudioManager.SetAudioState(0);
        yield return new WaitForSeconds(idleOffTime);

        tankAudioManager.SetAudioState(1);
        yield return new WaitForSeconds(idleOnTime);


        currentPhase = ProgressionPhase.Bridge;
        cameraController.SetParameters(bridgeCameraParameters, bridgeCameraTransitionParameters);
        knockoutTriggerGroups.ActivateOneDeactivateOthers((int)ProgressionPhase.Bridge);

        StartCoroutine(AccelerateTank(bridgeSpeed, bridgeAccelerationTime));
    }

}
