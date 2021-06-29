using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketRideItem : ExplosiveItem
{
    [Header("References")]
    public Transform modelTransform;
    public Rigidbody rocketRigidbody;
    [HideInInspector] public RocketRidePowerup powerup;


    [Header("Rocket Properties")]
    [SerializeField] private AnimationCurve rocketScaleCurve;
    [Space]
    [SerializeField] private float rocketLifetime;
    [Space]
    [SerializeField] private float chargeupRotationRate;
    [Space]
    [SerializeField] private float playerAttachedSpeed;
    [SerializeField] private float unburdenedSpeed;

    [SerializeField] private float accelerationRate;
    [Space]
    [SerializeField] private LayerMask explosionTriggerLayers;
    


    [HideInInspector] public bool isPlayerAttached = true;
    [HideInInspector] public bool canExplode = false;
    private Vector3 currentVelocity = Vector3.zero;




    void Awake()
    {
        StartCoroutine(QueueExplosion());
    }
    void FixedUpdate()
    {
        if(!isPlayerAttached){
            MoveRocket(Vector2.zero);
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if(canExplode){
            // Explode the rocket if it has hit a player other than the attached player OR solid terrain
            Hittable hittable = other.gameObject.GetComponent<Hittable>();

            if(hittable){
                // Only trigger explosion if the collision is not from the attached player
                if(hittable.player != powerup.attachedPowerupManager.player){
                    ExplodeRocket();
                }
            }
            else if(explosionTriggerLayers.ContainsLayer(other.gameObject.layer)){
                ExplodeRocket();
            }
        }
        
    }



    public IEnumerator ScaleModel(float duration){
        // Scales the rocket's model and collider over time
        float currentTime = 0;

        while(currentTime < duration){
            float timeGradient = currentTime / duration;

            float currentScale = rocketScaleCurve.Evaluate(timeGradient);

            modelTransform.localScale = currentScale * Vector3.one;

            currentTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        modelTransform.localScale = rocketScaleCurve.Evaluate(1) * Vector3.one;
    }



    public void MoveRocket(Vector2 playerInputVelocityLocal){
        // Moves the rocket's kinematic rigidbody at the current speed in the forward direction, plus extra forward and sideways player input
        Vector3 defaultVelocityWorld = isPlayerAttached ? playerAttachedSpeed * transform.forward : unburdenedSpeed * transform.forward;
        Vector3 inputVelocityWorld = (playerInputVelocityLocal.x * transform.right) + (playerInputVelocityLocal.y * transform.forward);
        Vector3 desiredVelocity = defaultVelocityWorld + inputVelocityWorld;

        Vector3 requiredVelocityChange = desiredVelocity - currentVelocity;
        Vector3 perFrameVelocityChange = accelerationRate * Time.fixedDeltaTime * requiredVelocityChange.normalized;

        Vector3 deltaV = Mathf.Min(requiredVelocityChange.magnitude, perFrameVelocityChange.magnitude) * requiredVelocityChange.normalized;

        currentVelocity += deltaV;

        rocketRigidbody.MovePosition(rocketRigidbody.position + (currentVelocity * Time.fixedDeltaTime));
    }


    public void RotateRocketDirection(Vector3 desiredDirection){
        // Linearly interpolates the rocket's rotation to look in the given direction
        
        Quaternion desiredRotation = Quaternion.LookRotation(desiredDirection, Vector3.up);

        Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, chargeupRotationRate * Time.fixedDeltaTime);

        rocketRigidbody.MoveRotation(newRotation);
    }


    private IEnumerator QueueExplosion(){
        yield return new WaitForSeconds(rocketLifetime);
        ExplodeRocket();
    }


    public void ExplodeRocket(){
        // Explode, but ignore attached player and instead perform custom behaviorfor them
        if(isPlayerAttached){
            Explode(rocketRigidbody.position, true, GetAttachedPlayer());
            powerup.OnExplosion();    
        }
        // No attached player, explode normally
        else{
            Explode(rocketRigidbody.position, true);
        }
    }

}
