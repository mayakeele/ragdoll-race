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
        End
    }


    [Header("References")]
    [SerializeField] private CameraController cameraController;
    [SerializeField] private TankAudioManager tankAudioManager;
    [SerializeField] private Rigidbody tankRigidbody;
    [SerializeField] private TreadsManager treadsManager;
    [SerializeField] private TurretManager turretManager;


    [Header("Phase Movement Properties")]
    [Space]
    [Header("Common")]
    [SerializeField] private float restAcceleration;
    [SerializeField] private float movingAcceleration;
    [Space]

    [Header("1 - Idle")]
    [SerializeField] private CameraParametersContainer idleCameraParameters;
    [SerializeField] private float idleOffTime;
    [SerializeField] private float idleOnTime;
    
    [Header("2 - Bridge")]
    [SerializeField] private CameraParametersContainer bridgeCameraParameters;
    [SerializeField] private float bridgeSpeed;

    [Header("3 - Desert")]
    [SerializeField] private float desertInitialX;
    [SerializeField] private CameraParametersContainer desertCameraParameters;
    [SerializeField] private float desertSpeed;

    [Header("4 - Pursuit")]
    [SerializeField] private float pursuitInitialX;
    [SerializeField] private CameraParametersContainer pursuitCameraParameters;
    [SerializeField] private float pursuitSpeed;

    [Header("5 - Canyon")]
    [SerializeField] private float canyonInitialX;
    [SerializeField] private CameraParametersContainer canyonCameraParameters;
    [SerializeField] private float canyonSpeed;

    [Header("6 - Boulders")]
    [SerializeField] private float bouldersInitialX;
    [SerializeField] private CameraParametersContainer bouldersCameraParameters;
    [SerializeField] private float bouldersSpeed;

    [Header("7 - Cliff")]
    [SerializeField] private float cliffInitialX;
    [SerializeField] private CameraParametersContainer cliffCameraParameters;
    [SerializeField] private float cliffSpeed;



    // Current State
    public float tankSpeed;
    private ProgressionPhase currentProgressionPhase = ProgressionPhase.Idle;



    void Start()
    {
        StartCoroutine(BeginLevel());
    }

    void FixedUpdate()
    {
        if(tankSpeed > 0){
            MoveTank(tankSpeed, Time.fixedDeltaTime);
        }


    }



    private void SetTankSpeed(float speed){
        tankSpeed = speed;
        treadsManager.SetTreadSpeed(speed);      
    }


    private void MoveTank(float speed, float timeDelta){
        // Since the tank uses a kinematic rigidbody, position is updated here
        Vector3 movementAmount = tankRigidbody.transform.forward * speed * timeDelta;

        tankRigidbody.MovePosition(tankRigidbody.position + movementAmount);
        tankRigidbody.velocity = tankRigidbody.transform.forward * speed;
    }


    private bool CheckIfNextPhaseStarts(ProgressionPhase currentPhase){
        // Checks whether the tank has crossed into the next phase's threshold given the current one
        
        switch(currentPhase){
            case ProgressionPhase.Idle:

            break;
        }
        return true;
    }



    private IEnumerator BeginLevel(){
        // Turns on the engine after a little while. Then waits a little longer, then starts moving and triggers the next phase

        currentProgressionPhase = ProgressionPhase.Idle;
        cameraController.SetParameters(idleCameraParameters);
        SetTankSpeed(0);


        tankAudioManager.SetAudioState(0);
        yield return new WaitForSeconds(idleOffTime);

        tankAudioManager.SetAudioState(1);
        yield return new WaitForSeconds(idleOnTime);


        currentProgressionPhase = ProgressionPhase.Bridge;
        cameraController.SetParameters(bridgeCameraParameters);
        SetTankSpeed(bridgeSpeed);
    }
}
