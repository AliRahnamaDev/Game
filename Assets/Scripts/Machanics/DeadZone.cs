using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Damageable dmg = other.GetComponent<Damageable>();
        if (dmg != null)
        {
            dmg.TakeDamage(dmg.health + 999); // kill instantly
        }
    }
}
