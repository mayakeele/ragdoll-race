using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirplaneStageManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CameraController cameraController;
    
    [Space]
    [SerializeField] private Material oceanMaterial;
    [SerializeField] private float oceanMeshLength;


    [Header("Shared Properties")]
    [SerializeField] private float accelerationTime;


    [Header("Per-Phase Properties")]
    [SerializeField] private List<CameraParametersContainer> cameraParametersContainers;
    [SerializeField] private List<CameraTransitionParameters> cameraParameterTransitionContainers;
    [Space]
    [SerializeField] private List<GameObject> knockoutTriggerGroups;
    [Space]
    [SerializeField] private float[] phaseDurations;
    [SerializeField] private float[] phaseAirplaneSpeeds;



    private int currentPhase = 0;
    private int totalNumPhases;

    [HideInInspector] public float currentAirplaneSpeed;


    void Awake()
    {
        totalNumPhases = phaseDurations.Length;
    }


    void Start()
    {
        currentAirplaneSpeed = phaseAirplaneSpeeds[0];
        StartCoroutine(ActivatePhase(0));
    }


    void Update()
    {
        oceanMaterial.ScrollTexture(0, -currentAirplaneSpeed / oceanMeshLength, Time.deltaTime);
    }



    private IEnumerator ActivatePhase(int newPhaseNum){
        // Sets the current phase to the new number, then updates camera parameters and sets a timer counting down to the next phase

        currentPhase = newPhaseNum;

        // Only try to transition to new parameters if the parameters and transition exist
        CameraParametersContainer newParams = cameraParametersContainers[currentPhase];
        CameraTransitionParameters transitionParams = cameraParameterTransitionContainers[currentPhase];

        if(newParams && transitionParams){
            cameraController.SetParameters(newParams, transitionParams);
        }
        else if(newParams){
            cameraController.SetParameters(newParams);
        }


        // Activate this phase's KO Trigger, deactivate others
        knockoutTriggerGroups.ActivateOneDeactivateOthers(newPhaseNum);


        // Set airplane speed
        StartCoroutine(AcceleratePlane(phaseAirplaneSpeeds[currentPhase], accelerationTime));

        
        // Set a timer to wait out the phase
        float waitTime = phaseDurations[currentPhase];
        yield return new WaitForSeconds(waitTime);

        // Activate next phase if there is a phase to activate; otherwise ends the chain
        int nextPhase = currentPhase + 1;
        if(nextPhase < totalNumPhases){
            StartCoroutine(ActivatePhase(nextPhase));
        }
    }


    private IEnumerator AcceleratePlane(float finalSpeed, float timePeriod){
        // Accelerates the tank from its current speed to the given speed, over a given period of time

        float initialSpeed = currentAirplaneSpeed;

        float currTime = 0;
        while(currTime < timePeriod){
            currentAirplaneSpeed = currTime.Map(0, timePeriod, initialSpeed, finalSpeed);

            currTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        currentAirplaneSpeed = finalSpeed;
    }

}
