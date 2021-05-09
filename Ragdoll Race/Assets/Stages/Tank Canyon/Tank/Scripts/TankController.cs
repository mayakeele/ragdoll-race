using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody tankRigidbody;
    [SerializeField] private TreadsManager treadsManager;
    [SerializeField] private TurretManager turretManager;


    [Header("Movement Properties")]
    [SerializeField] private float tankSpeed;



    void Start()
    {
        treadsManager.SetTreadSpeed(tankSpeed);
    }


    void Update()
    {
        
    }
}
