using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BanhammerEntity : ExplosiveSpawnedEntity
{

    [Header("References")]
    public Rigidbody rigidbody;
    public Transform handleTransform;


    [Header("Hit Properties")]
    [SerializeField] private AnimationCurve minSpeedToKillAtPercent;


    [Header("Effects")]
    [SerializeField] private Material hammerMaterial;
    [SerializeField] private float colorCycleSpeed;
    [Space]
    [SerializeField] private GameObject instakillEffect;


    
    void Awake()
    {
        StartCoroutine(CycleColor());
    }



    void OnCollisionEnter(Collision other)
    {
        Debug.Log("hit");
        Player hitPlayer = other.gameObject.GetComponent<Hittable>()?.player;

        if(hitPlayer && hitPlayer != GetAttachedPlayer()){
            // If the relative and absolute speed of the hammer is greater than the threshold speed at the hit player's percent, kills instantly
            float thresholdSpeed = minSpeedToKillAtPercent.Evaluate(hitPlayer.currentDamage);

            if(rigidbody.velocity.magnitude >= thresholdSpeed && other.relativeVelocity.magnitude > thresholdSpeed){
                hitPlayer.TryKnockout(instakillEffect);
            }
        }
    }



    private IEnumerator CycleColor(){
        // Cycles the hammer material's hue continuously over time

        Color.RGBToHSV(hammerMaterial.color, out float hBase, out float sBase, out float vBase);
        Color.RGBToHSV(hammerMaterial.GetColor("_EmissionColor"), out float hEmission, out float sEmission, out float vEmission);


        while(true){
            hBase += (colorCycleSpeed * Time.deltaTime);
            hBase %= 1;

            hammerMaterial.color = Color.HSVToRGB(hBase, sBase, vBase);
            hammerMaterial.SetColor("_EmissionColor", Color.HSVToRGB(hBase, sEmission, vEmission));

            yield return new WaitForEndOfFrame();
        }
    }
}
