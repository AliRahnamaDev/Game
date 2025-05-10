using System;
using UnityEngine;

public class Trap_Fire : MonoBehaviour
{
    Animator animator;
    public BoxCollider2D boxCollider1;
    public BoxCollider2D boxCollider2;

    private void Start()
    {
        animator = GetComponent<Animator>();
        boxCollider1 = GetComponent<BoxCollider2D>();
        boxCollider2 = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        
    }

    public void OnAnimation()
    {
        animator.SetBool("active",true);
        boxCollider1.enabled=true;
    }
    
    public void OffAnimation()
    {
        animator.SetBool("active",false);
        boxCollider1.enabled=false;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        
    }
}
