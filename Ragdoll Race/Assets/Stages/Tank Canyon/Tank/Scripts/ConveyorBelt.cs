using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    private Rigidbody beltRigidbody;

    [HideInInspector] public float speed;


    private void Awake()
    {
        beltRigidbody = GetComponent<Rigidbody>();
    }


    void FixedUpdate()
    {
        Vector3 displacement = transform.forward * speed * Time.fixedDeltaTime;

        beltRigidbody.position -= displacement;
        beltRigidbody.MovePosition(beltRigidbody.position + displacement);
    }

}
