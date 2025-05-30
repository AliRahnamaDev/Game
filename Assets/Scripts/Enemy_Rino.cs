using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Rino : Enemy
{
    [Header("RinoDetails")]  [SerializeField]
    private Vector2 impactPower;
    [SerializeField]private float detectionRange;
    private bool playerDetected = false;
    public float ChargeTime = 0.5f;

    protected override void Update()
    {
        if (rb.velocity.x != 0)
        {
            animator.SetBool("isRunning",true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
        base.Update();
        HandleCollisions();
        HandleCharge();
    }
    public override void HandleCollisions()
    {
        base.HandleCollisions();
        playerDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDirection, detectionRange, whatIsPlayer);
        if(playerDetected)
            canMove = true;
    }

    private void HandleCharge()
    {
        if (canMove == false)
            return;
        rb.velocity = new Vector2(facingDirection * speed, rb.velocity.y);
        if (isWallDetected)
        {
            WallHit();
        }
        if (!isGroundInfrontDetected)
        {
            canMove = false;
            rb.velocity = Vector2.zero;
            Flip();
        }
    }

    private void WallHit()
    {
        StartCoroutine(AnimateRinoHit());
        animator.SetBool("HitWall",true);
        rb.velocity = new Vector2(impactPower.x  *  -facingDirection, rb.velocity.y);
        ChargeOver();
    }

    void  ChargeOver() => canMove = false;
    
    IEnumerator AnimateRinoHit()
    {
        animator.SetBool("HitWall",true);
        yield return new WaitForSeconds(ChargeTime);
        animator.SetBool("HitWall",false);
    }
    
}
