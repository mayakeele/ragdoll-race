using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankAudioManager : MonoBehaviour
{
    [Header("Engine")]
    [SerializeField] private AudioSource engineAudioSource;
    [Range(0,1)] [SerializeField] private float engineVolume;
    [SerializeField] private AudioClip engineIdleSound;
    [SerializeField] private AudioClip engineSlowSound;
    [SerializeField] private AudioClip engineMediumSound;
    [SerializeField] private AudioClip engineFastSound;


    [Header("Treads")]
    [SerializeField] private AudioSource treadsAudioSource;
    [Range(0,1)] [SerializeField] private float treadsVolume;
    [SerializeField] private AudioClip treadsSlowSound;
    [SerializeField] private AudioClip treadsMediumSound;
    [SerializeField] private AudioClip treadsFastSound;


    [Header("Turret")]
    [SerializeField] private AudioSource turretAudioSource;
    [Range(0,1)] [SerializeField] private float turretVolume;
    [SerializeField] private AudioClip turretTurnSound;
    [SerializeField] private AudioClip turretFireSound;



    public void SetAudioState(int speedLevel){
        // 0 = off, 1 = idle, 2 = slow, 3 = medium, 4 = fast

        switch (speedLevel){
            case 0:
                engineAudioSource.StopAudio();
                treadsAudioSource.StopAudio();
            break;


            case 1:
                engineAudioSource.PlayLoop(engineIdleSound, engineVolume);
                treadsAudioSource.StopAudio();
            break;

            case 2:
                engineAudioSource.PlayLoop(engineSlowSound, engineVolume);
                treadsAudioSource.PlayLoop(treadsSlowSound, treadsVolume);
            break;

            case 3:
                engineAudioSource.PlayLoop(engineMediumSound, engineVolume);
                treadsAudioSource.PlayLoop(treadsMediumSound, treadsVolume);
            break;

            case 4:
                engineAudioSource.PlayLoop(engineFastSound, engineVolume);
                treadsAudioSource.PlayLoop(treadsFastSound, treadsVolume);
            break;


            default:
                engineAudioSource.StopAudio();
                treadsAudioSource.StopAudio();
            break;
        }
    }
}
