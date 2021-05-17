using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    private Rigidbody beltRigidbody;
    private MovingPlatform movingPlatform;

    [HideInInspector] public float speed;


    private void Awake()
    {
        beltRigidbody = GetComponent<Rigidbody>();
        movingPlatform = GetComponent<MovingPlatform>();
    }


    void FixedUpdate()
    {
        // Shift the "conveyor belt" forward, then teleport it back
        Vector3 displacement = transform.forward * speed * Time.fixedDeltaTime;

        beltRigidbody.position -= displacement;
        beltRigidbody.MovePosition(beltRigidbody.position + displacement);

        // Set the moving platform component's velocity
        movingPlatform.velocity = transform.forward * speed;
    }

}
