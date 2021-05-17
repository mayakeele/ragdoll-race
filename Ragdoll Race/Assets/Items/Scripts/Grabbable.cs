using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour
{
    [Header("Grab Properties")]
    [SerializeField] private bool twoHandedOnly;
    public bool canGrabAnywhere;
    public List<Transform> grabTargetPoints;



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
