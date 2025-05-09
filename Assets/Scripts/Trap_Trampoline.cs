using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Trap_Trampoline : MonoBehaviour
{
    private Animator animator;
    public float pushForce =5;
    

    private void Awake()
    {
        
        animator = GetComponent<Animator>();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        if (player != null)
        {
            if (other.tag == "Player")
            {
                StartCoroutine(Trampoline());
                player.push(pushForce);
            }
            
        }
    }

    private IEnumerator Trampoline()
    {
        animator.SetBool("isActive", true);
        yield return new WaitForSecondsRealtime(0.5f);
        animator.SetBool("isActive", false);
       
    }
}
