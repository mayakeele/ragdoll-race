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
    [SerializeField] private CameraController cameraController;
    [SerializeField] private TankAudioManager tankAudioManager;
    [SerializeField] private TreadsManager treadsManager;
    [SerializeField] private TurretManager turretManager;


    [Header("Phase Movement Properties")]
    [Space]

    [Header("1 - Idle")]
    [SerializeField] private CameraParametersContainer idleCameraParameters;
    [SerializeField] private float idleOffTime;
    [SerializeField] private float idleOnTime;
    
    [Header("2 - Bridge")]
    [SerializeField] private CameraParametersContainer bridgeCameraParameters;
    [SerializeField] private float bridgeSpeed;
    [SerializeField] private float bridgeAccelerationTime;
    [SerializeField] private float bridgeFinalDistance;

    [Header("3 - Desert")]
    [SerializeField] private CameraParametersContainer desertCameraParameters;
    [SerializeField] private float desertSpeed;
    [SerializeField] private float desertAccelerationTime;
    [SerializeField] private float desertFinalDistance;

    [Header("4 - Pursuit")]
    [SerializeField] private CameraParametersContainer pursuitCameraParameters;
    [SerializeField] private float pursuitSpeed;
    [SerializeField] private float pursuitAccelerationTime;
    [SerializeField] private float pursuitFinalDistance;

    [Header("5 - Canyon")]
    [SerializeField] private CameraParametersContainer canyonCameraParameters;
    [SerializeField] private float canyonSpeed;
    [SerializeField] private float canyonAccelerationTime;
    [SerializeField] private float canyonFinalDistance;

    [Header("6 - Boulders")]
    [SerializeField] private CameraParametersContainer bouldersCameraParameters;
    [SerializeField] private float bouldersSpeed;
    [SerializeField] private float bouldersAccelerationTime;
    [SerializeField] private float bouldersFinalDistance;

    [Header("7 - Cliff")]
    [SerializeField] private CameraParametersContainer cliffCameraParameters;
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

            case ProgressionPhase.Idle:

            break;

            case ProgressionPhase.Bridge:
                if(distanceCovered > bridgeFinalDistance){
                    currentPhase = ProgressionPhase.Desert;

                    StartCoroutine(AccelerateTank(desertSpeed, desertAccelerationTime));
                }
            break;

            case ProgressionPhase.Desert:
                if(distanceCovered > desertFinalDistance){
                    currentPhase = ProgressionPhase.Pursuit;

                    StartCoroutine(AccelerateTank(pursuitSpeed, pursuitAccelerationTime));
                }
            break;

            case ProgressionPhase.Pursuit:
                if(distanceCovered > pursuitFinalDistance){
                    currentPhase = ProgressionPhase.Canyon;

                    StartCoroutine(AccelerateTank(canyonSpeed, canyonAccelerationTime));
                }
            break;

            case ProgressionPhase.Canyon:
                if(distanceCovered > canyonFinalDistance){
                    currentPhase = ProgressionPhase.Boulders;

                    StartCoroutine(AccelerateTank(bouldersSpeed, bouldersAccelerationTime));
                }
            break;

            case ProgressionPhase.Boulders:
                if(distanceCovered > bouldersFinalDistance){
                    currentPhase = ProgressionPhase.Cliff;

                    StartCoroutine(AccelerateTank(cliffSpeed, cliffAccelerationTime));
                }
            break;

            case ProgressionPhase.Cliff:
                if(distanceCovered > cliffFinalDistance){
                    
                }
            break;
        }

    }


    private IEnumerator AccelerateTank(float finalSpeed, float timePeriod){
        // Accelerates the tank from its current speed to the given speed, over a given period of time

        float initialSpeed = tankSpeed;

        float currTime = 0;
        while(currTime < timePeriod){
            SetTankSpeed(currTime.MapClamped(0, timePeriod, initialSpeed, finalSpeed));

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


        tankAudioManager.SetAudioState(0);
        yield return new WaitForSeconds(idleOffTime);

        tankAudioManager.SetAudioState(1);
        yield return new WaitForSeconds(idleOnTime);


        currentPhase = ProgressionPhase.Bridge;
        cameraController.SetParameters(bridgeCameraParameters);

        StartCoroutine(AccelerateTank(bridgeSpeed, bridgeAccelerationTime));
    }
}
