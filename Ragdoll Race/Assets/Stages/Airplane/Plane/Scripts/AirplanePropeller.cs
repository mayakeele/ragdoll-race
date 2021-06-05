using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirplanePropeller : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Rigidbody propellerRigidbody;


    [Header("Physics Properties")]
    [SerializeField] private float rotationsPerSecond;
    [SerializeField] private bool flipDirection;


    // Start is called before the first frame update
    void Start()
    {
        propellerRigidbody.maxAngularVelocity = rotationsPerSecond * Mathf.PI / 2;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Quaternion rotationAmount = Quaternion.AngleAxis(rotationsPerSecond * 360 * Time.fixedDeltaTime, transform.forward * (flipDirection ? -1 : 1));
        propellerRigidbody.MoveRotation(propellerRigidbody.rotation * rotationAmount);
    }
}
