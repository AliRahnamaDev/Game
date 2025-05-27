using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageScript : MonoBehaviour
{
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        if (player !=null)
        {
            player.KnockBack(transform.position.x);
            Debug.Log("KnockBack");
        }
        
    }
}
