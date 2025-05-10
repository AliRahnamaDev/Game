using System;
using UnityEngine;

public class Trap_Spikeball : MonoBehaviour
{
   private Rigidbody2D rb;
   public float force;

   private void Start()
   {
      rb = GetComponent<Rigidbody2D>();
      Vector2 pushVector = new Vector2(force, 0);
      rb.AddForce(pushVector, ForceMode2D.Impulse);
   }
   
}
