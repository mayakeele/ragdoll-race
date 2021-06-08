using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera camera;
    private FastNoiseLite noise;


    [Header("Rotational Shake Amplitudes")]
    [SerializeField] private float pitchAmplitudeMax;
    [SerializeField] private float yawAmplitudeMax;
    [SerializeField] private float rollAmplitudeMax;
    [Space]

    [Header("Translational Shake Amplitudes")]
    [SerializeField] private float xAmplitudeMax;
    [SerializeField] private float yAmplitudeMax;
    [SerializeField] private float zAmplitudeMax;
    [Space]

    [Header("Other Properties")]
    [SerializeField] private float rotationNoiseFrequencyMultiplier;
    [SerializeField] private float translationNoiseFrequencyMultiplier;
    [SerializeField] private float traumaDecayRate;

    private float currentTrauma = 0;
    private int seed;



    void Awake()
    {
        seed = Random.Range(1, 1000);
        noise = new FastNoiseLite(seed);
        noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
    }


    private void Update()
    {
        DecayTrauma(Time.deltaTime);

        if(currentTrauma > 0){
            MoveCamera(Time.time);
        }
        else{
            camera.transform.localRotation = Quaternion.identity;
        }
    }



    public void AddCameraShake(float traumaAmount){
        AddTrauma(traumaAmount);
    }



    private void MoveCamera(float currentTime){
        // Shakes the camera continuously by rotating in 3 dimensions using Perlin noise

        float shakeAmount = Mathf.Pow(currentTrauma, 2);

        // Shake Rotationally
        noise.SetSeed(seed);
        float pitchAngle = shakeAmount * pitchAmplitudeMax * noise.GetNoise(currentTime * rotationNoiseFrequencyMultiplier, 0);
        noise.SetSeed(seed+1);
        float yawAngle = shakeAmount * yawAmplitudeMax * noise.GetNoise(currentTime * rotationNoiseFrequencyMultiplier, 0);
        noise.SetSeed(seed+2);
        float rollAngle = shakeAmount * rollAmplitudeMax * noise.GetNoise(currentTime * rotationNoiseFrequencyMultiplier, 0);

        // Shake Translationally
        noise.SetSeed(seed+3);
        float xDisplacement = shakeAmount * xAmplitudeMax * noise.GetNoise(currentTime * translationNoiseFrequencyMultiplier, 0);
        noise.SetSeed(seed+4);
        float yDisplacement = shakeAmount * yAmplitudeMax * noise.GetNoise(currentTime * translationNoiseFrequencyMultiplier, 0);
        noise.SetSeed(seed+5);
        float zDisplacement = shakeAmount * zAmplitudeMax * noise.GetNoise(currentTime * translationNoiseFrequencyMultiplier, 0);

        // Apply rotation and translation shake
        Quaternion newRotation = Quaternion.Euler(pitchAngle, yawAngle, rollAngle);
        Vector3 newPosition = new Vector3(xDisplacement, yDisplacement, zDisplacement);
        camera.transform.localRotation = newRotation;
        camera.transform.localPosition = newPosition;
    }


    private void DecayTrauma(float timeStep){
        // Linearly decrements trauma and clamps it between 0 and 1

        currentTrauma -= traumaDecayRate * timeStep;
        currentTrauma = Mathf.Clamp(currentTrauma, 0, 1);
    }


    private void AddTrauma(float trauma){
        currentTrauma += trauma;
        currentTrauma = Mathf.Clamp(currentTrauma, 0, 1);
    }
}
