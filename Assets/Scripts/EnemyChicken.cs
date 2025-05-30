using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChicken : Enemy
{
    [Header("Chicken details")]
    [SerializeField] private float aggroDuration;
    private float aggroTimer;
    [SerializeField] private bool playerDetection;
    [SerializeField] private float detectionRange;
    [SerializeField] private bool canFlip = true;
    public float TimerToChickenBack;
    private BoxCollider2D boxCollider;
    protected override void Awake()
    {
        base.Awake();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    protected override void Update()
    {
        aggroTimer -= Time.deltaTime;
        base.Update();
        if (isDead)
        {
            animator.SetTrigger("isDead");
            return;
        }if(playerDetection)
        {
            canMove = true;
            aggroTimer = aggroDuration;
        }
        if(aggroTimer <= 0)
            canMove = false;
        AnimateMovement();
        base.Update() ;
        HandleMovement() ;
        HandleCollisions() ;
        HandleTurnAround();
    }

    private void HandleTurnAround()
    {
        if (!isGroundInfrontDetected || isWallDetected)
        {
            if(isGrounded ==false)
                return;
            Flip() ;
            canMove = false;
            rb.velocity = Vector2.zero ;
            
        }
    }

    private void HandleMovement()
    {
        if(canMove == false)
            return;
        float xValue = player.transform.position.x;
        HandleFlip(xValue);
        rb.velocity = new Vector2(speed*facingDirection, rb.velocity.y);
    }

    protected override void HandleFlip(float xValue)
    {
        if (xValue > transform.position.x && !facingRight || xValue < transform.position.x && facingRight)
        {
            if (canFlip)
            {
                canFlip = false;
                Invoke(nameof(Flip), TimerToChickenBack);
            }
        }
    }
    protected override void Flip()
    {
        base.Flip();
        canFlip = true;
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

    public override void HandleCollisions()
    {
        base.HandleCollisions();
        playerDetection = Physics2D.Raycast(transform.position,Vector2.right * facingDirection, detectionRange,whatIsPlayer);
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + facingDirection * detectionRange, transform.position.y));
    }
}
