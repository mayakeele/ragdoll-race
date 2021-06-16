using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupPickup : MonoBehaviour
{
    [Header("Physical Pickup Zone")]
    [SerializeField] private Collider pickupTrigger;

    [Header("Powerups Available")]
    [SerializeField] private List<Powerup> availablePowerups;
}
