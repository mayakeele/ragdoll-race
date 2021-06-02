using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreadsManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<ConveyorBelt> treadSegments;
    [SerializeField] private Material treadMaterial;
    [SerializeField] private TankController tankController;


    [Header("Tread Properties")]
    [SerializeField] private float totalTreadLength;
    private float relativeTreadSpeed;



    void Start()
    {

    }

    void Update()
    {
        treadMaterial.ScrollTexture(0, relativeTreadSpeed / totalTreadLength, Time.deltaTime);
    }

    
    
    public void SetTreadSpeed(float speed){
        relativeTreadSpeed = speed;

        foreach(ConveyorBelt segment in treadSegments){
            segment.speed = relativeTreadSpeed;
        }
    }
}
