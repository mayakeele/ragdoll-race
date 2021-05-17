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
        float deltaOffset = (relativeTreadSpeed / totalTreadLength) * Time.deltaTime * treadMaterial.mainTextureScale.y;
        float newOffset = treadMaterial.mainTextureOffset.y + deltaOffset;
        
        treadMaterial.mainTextureOffset = new Vector2(0, newOffset % 1);
    }

    
    
    public void SetTreadSpeed(float speed){
        relativeTreadSpeed = speed;

        foreach(ConveyorBelt segment in treadSegments){
            segment.speed = relativeTreadSpeed;
        }
    }
}
