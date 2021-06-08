using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroyer : MonoBehaviour
{
    public float lifeDuration;

    void Start()
    {
        Destroy(this.gameObject, lifeDuration);
    }
}
