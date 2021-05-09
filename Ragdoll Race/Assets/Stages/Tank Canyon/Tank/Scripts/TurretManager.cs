using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody turretRigidbody;


    [Header("Rotation Properties")]
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float timeBetweenMoves;



    // Private Variables
    private float currentAngle = 0;
    private float targetAngle = 0;
    private float degreesPerFrame;



    void Start()
    {
        degreesPerFrame = rotationSpeed * Time.fixedDeltaTime;
        
        StartCoroutine(SetRandomAngle(timeBetweenMoves));
    }

    void FixedUpdate()
    {
        float angleDifference = FloatExtensions.AngleDifference(currentAngle, targetAngle);

        if(Mathf.Abs(angleDifference) > degreesPerFrame){
            // Rotate turret towards target
            currentAngle += degreesPerFrame * Mathf.Sign(angleDifference);
            currentAngle %= 360;

            Quaternion newRotation = Quaternion.Euler(0, currentAngle, 0);
            turretRigidbody.MoveRotation(newRotation);
        }
    }



    private IEnumerator SetRandomAngle(float waitTime){
        targetAngle = Random.Range(0,360);
        yield return new WaitForSeconds(waitTime);
        StartCoroutine(SetRandomAngle(waitTime));
        yield break;
    }
}
