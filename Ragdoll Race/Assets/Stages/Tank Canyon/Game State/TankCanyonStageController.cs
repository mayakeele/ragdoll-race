using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankCanyonStageController : MonoBehaviour
{
    [Header("Event Timing")]
    [Space]
    [Header("1 - Start")]
    [SerializeField] private float startIdleTime;

    [Header("2 - Bridge")]
    [SerializeField] private float bridgeAccelerationTime;
    [SerializeField] private float bridgeTime;

    [Header("3 - Desert")]
    [SerializeField] private float desertAccelerationTime;
    [SerializeField] private float desertTime;

    [Header("4 - Pursuit")]
    [SerializeField] private float pursuitAccelerationTime;
    [SerializeField] private float pursuitTime;

    [Header("5 - Canyon")]
    [SerializeField] private float canyonAccelerationTime;
    [SerializeField] private float canyonTime;

    [Header("6 - Rocks")]
    [SerializeField] private float rocksAccelerationTime;
    [SerializeField] private float rocksTime;





    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
