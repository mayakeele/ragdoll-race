using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollCannon : MonoBehaviour
{
    public GameObject ragdollPrefab;
    public float speed;
    public float createPeriod;


    private float timer = 0;
    void Start()
    {
        timer = createPeriod - 0.05f;
    }

    // Update is called once per frame
    void Update()
    {
        if(timer >= createPeriod){
            timer = 0;

            Quaternion rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
            Vector3 velocity = transform.forward * Random.Range(0, speed);

            GameObject ragdoll = Instantiate(ragdollPrefab, transform.position, rotation);

            ragdoll.GetComponent<Ragdoll>().rootRigidbody.AddForce(velocity, ForceMode.VelocityChange);
        }

        timer += Time.deltaTime;
    }
}
