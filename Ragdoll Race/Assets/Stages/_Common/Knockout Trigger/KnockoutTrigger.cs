using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockoutTrigger : MonoBehaviour
{
    public Collider collider;

    private void Awake()
    {
        if(!collider){
            collider = GetComponent<Collider>();
        }
    }


    
    private void OnTriggerEnter(Collider other)
    {
        Hittable hittable = other.gameObject.GetComponent<Hittable>();

        if(hittable){
            Player player = hittable.player;

            player.Kill();
        }
    }
}
