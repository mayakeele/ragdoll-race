using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera camera;
    private FastNoiseLite noise;


    [Header("Shake Properties")]
    [SerializeField] private float pitchAmplitudeMax;
    [SerializeField] private float yawAmplitudeMax;
    [SerializeField] private float rollAmplitudeMax;
    [Space]
    [SerializeField] private float noiseFrequencyMultiplier;
    [Space]
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
            RotateCamera(Time.time);
        }
        else{
            camera.transform.localRotation = Quaternion.identity;
        }
    }



    public void AddCameraShake(float traumaAmount){
        AddTrauma(traumaAmount);
    }



    private void RotateCamera(float currentTime){
        // Shakes the camera continuously by rotating in 3 dimensions using Perlin noise

        float shakeAmount = Mathf.Pow(currentTrauma, 2);

        noise.SetSeed(seed);
        float pitchAngle = shakeAmount * pitchAmplitudeMax * noise.GetNoise(currentTime * noiseFrequencyMultiplier, 0);
        noise.SetSeed(seed+1);
        float yawAngle = shakeAmount * yawAmplitudeMax * noise.GetNoise(currentTime * noiseFrequencyMultiplier, 0);
        noise.SetSeed(seed+2);
        float rollAngle = shakeAmount * rollAmplitudeMax * noise.GetNoise(currentTime * noiseFrequencyMultiplier, 0);

        Quaternion newRotation = Quaternion.Euler(pitchAngle, yawAngle, rollAngle);
        camera.transform.localRotation = newRotation;
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
