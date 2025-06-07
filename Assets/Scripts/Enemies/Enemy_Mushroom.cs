using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy_Mushroom : Enemy
{
    private BoxCollider2D boxCollider;
    protected override void Awake()
    {
      base.Awake();
      boxCollider = GetComponent<BoxCollider2D>();
    }

    protected override void Update()
    {
        base.Update();
        if(isDead)
            return;
        AnimateMovement();
        base.Update() ;
        HandleMovement() ;
        HandleCollisions() ;
        if (!isGroundInfrontDetected || isWallDetected)
        {
            if(isGrounded ==false)
                return;
            idleTimer = idleDuration ;
            rb.velocity = Vector2.zero ;
            Flip() ;
        }
    }
    private void HandleMovement()
    {
        if(idleTimer > 0)
            return;
        rb.velocity = new Vector2(speed*facingDirection, rb.velocity.y);
    }

    private void AnimateMovement()
    {
        if (rb.velocity.x != 0)
        {
            animator.SetBool("isRunning",true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
    }
}