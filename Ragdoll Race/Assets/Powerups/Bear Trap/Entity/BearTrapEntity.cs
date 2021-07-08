using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearTrapEntity : SpawnedEntity
{
    [Header("Part References")]
    [SerializeField] private GameObject leftJaw;
    [SerializeField] private GameObject rightJaw;
    private Collider triggerCollider;


    [Header("Timing")]
    [SerializeField] private float disarmedDuration;
    [SerializeField] private float reopenDuration;
    [SerializeField] private float snapShutDuration;
    [SerializeField] private float trappedDuration;


    [Header("Trapping Properties")]
    [SerializeField] private LayerMask triggerableLayers;
    [SerializeField] private int numActivationsRemaining = 2;
    [Space]
    [SerializeField] private float damageDealt;
    [SerializeField] private float jointStrength;


    [Header("Effects")]
    [SerializeField] private float openAngle;
    [SerializeField] private float closedAngle;
    [SerializeField] private AnimationCurve reopenCurve;
    [SerializeField] private AnimationCurve snapShutCurve;
    [Space]
    [SerializeField] private GameObject reopenEffect;
    [SerializeField] private GameObject snapShutEffect;



    private bool isArmed = false;



    void Awake()
    {
        triggerCollider = GetComponent<Collider>();
    }

    void Start()
    {
        StartCoroutine(DisarmWaitThenArm());
    }

    
    void OnTriggerStay(Collider other)
    {
        if(isArmed && triggerableLayers.ContainsLayer(other.gameObject.layer)){
            Hittable hittable = other.gameObject.GetComponent<Hittable>();
            if(hittable) StartCoroutine(SnapShut(hittable));
        }
    }



    private IEnumerator DisarmWaitThenArm(){
        // Performs arming sequence including initial closed delay and opening animation

        // Start out closed and disarmed
        isArmed = false;
        SetJawsAngle(closedAngle);
        yield return new WaitForSeconds(disarmedDuration);

        // Perform opening animation over time
        float currentTime = 0;
        while(currentTime < reopenDuration){
            float timeGradient = currentTime / reopenDuration;
            float angleGradient = reopenCurve.Evaluate(timeGradient);

            float newAngle = angleGradient.MapPercent(closedAngle, openAngle);
            SetJawsAngle(newAngle);

            currentTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        // Trap is fully armed now
        SetJawsAngle(openAngle);
        isArmed = true;
    }


    private IEnumerator SnapShut(Hittable trappedHittable){
        // Performs closing animation and post-close delay and release
        // Applies damage and creates a joint between the trap and the trapped player's collider

        isArmed = false;
        numActivationsRemaining--;

        // Create a fixed joint on the trapped body part, anchoring it in whatever position it was in
        FixedJoint joint = trappedHittable.gameObject.AddComponent<FixedJoint>();
        joint.enableCollision = false;
        joint.enablePreprocessing = false;

        // Apply damage to the player
        trappedHittable.Hit(triggerCollider.transform.position, Vector3.zero, damageDealt, 0, 1, GetAttachedPlayer());

        // Spawn closing effects


        // Animate jaws closing over time
        float currentTime = 0;
        while(currentTime < snapShutDuration){
            float timeGradient = currentTime / snapShutDuration;
            float angleGradient = snapShutCurve.Evaluate(timeGradient);

            float newAngle = angleGradient.MapPercent(openAngle, closedAngle);
            SetJawsAngle(newAngle);

            currentTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        SetJawsAngle(closedAngle);


        // Keep the player trapped for a while, wait
        yield return new WaitForSeconds(trappedDuration);


        // Release player, play release effects and start disarm sequence
        Destroy(joint);
        // ~~~~~~~~~~~~~~~~~~ effects here


        // If this was the final activation, destroy self
        if(numActivationsRemaining <= 0){
            Destroy(this.gameObject);
        }
        // Otherwise, rearm the trap
        else{
            StartCoroutine(DisarmWaitThenArm());
        }   
    }



    private void SetJawsAngle(float angle){
        // Sets both jaws' z axis angle to the given angle
        leftJaw.transform.localRotation = Quaternion.Euler(0, 0 , angle);
        rightJaw.transform.localRotation = Quaternion.Euler(0, 180 , angle);
    }
}
