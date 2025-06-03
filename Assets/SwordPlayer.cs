using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordPlayer : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    
    [Header("Movement")]
    public float walkSpeed = 2.5f;
    public float runSpeed = 6f;
    public float jumpForce = 10f;
    public bool canDoubleJump = true;
    
    [Header("Status")]
    public bool canFlip = true;
    public bool isGrounded;
    public bool isJumping;
    public bool isRunning;
    public bool isWalking;
    public bool isFalling;
    public bool isFacingRight = true;
    public float facingDirection = 1;
    public bool isAttacking;

    [Header("Layers")] 
    public float groundCheckDistance;
    public LayerMask whatIsGround;


    public void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Attack();
        }
        DetectGround();
        Movement();
        HandleJump();
        setStatus();
        Animate();
        HandleFlip();
    }

    public void Attack()
    {
        //TODO
    }

    public void Movement()
    {
        float moveInput = 0f;

        if (Input.GetKey(KeyCode.D))
            moveInput = 1f;
        else if (Input.GetKey(KeyCode.A))
            moveInput = -1f;

        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
    }

    public void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    public void HandleJump()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    public void setStatus()
    {
        if (Mathf.Abs(rb.velocity.x) > 0 && Mathf.Abs(rb.velocity.x) < runSpeed) isWalking = true; else isWalking = false;
        
        if (Mathf.Abs(rb.velocity.x) >= runSpeed) isRunning = true; else isRunning = false;

        if (rb.velocity.y > 0.1) isJumping = true; else isJumping = false;
        if(isJumping) {isRunning = false; isWalking = false;}
        
        if(rb.velocity.y < -0.1) isFalling = true; else isFalling = false;
        if(isFalling) {isRunning = false; isWalking = false;}
        
        if (Input.GetMouseButton(0)) isAttacking = true; else isAttacking = false;
        
    }

    public void Animate()
    {
        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isJumping", isJumping);
        animator.SetBool("isFalling", isFalling);
        animator.SetBool("isAttacking",isAttacking);
    }

    public void HandleFlip()
    {
        if (Input.GetKey(KeyCode.D) && !isFacingRight)
            StartCoroutine(Flip());
        else if (Input.GetKey(KeyCode.A) && isFacingRight)
           StartCoroutine(Flip());
    }

    IEnumerator Flip()
    {
        isFacingRight = !isFacingRight;
        facingDirection = isFacingRight ? 1 : -1;
        transform.Rotate(0f, 180f, 0f);
        canFlip = false;
        yield return new WaitForSeconds(0.1f);
        canFlip = true;
    }
    
    public void DetectGround()
    {
        Vector2 origin = new Vector2((transform.position.x - 0.5f), transform.position.y - 0.5f);
        Vector2 origin2 = new Vector2((transform.position.x + 0.5f), transform.position.y - 0.5f);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, whatIsGround);
        RaycastHit2D hit2 = Physics2D.Raycast(origin2, Vector2.down, groundCheckDistance, whatIsGround);
        isGrounded = hit.collider != null || hit2.collider != null;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 origin = new Vector2((transform.position.x - 0.5f), transform.position.y - 0.5f);
        Vector2 origin2 = new Vector2((transform.position.x + 0.5f), transform.position.y - 0.5f);
        Gizmos.DrawLine(origin, origin + Vector2.down * groundCheckDistance);
        Gizmos.DrawLine(origin2, origin2 + Vector2.down * groundCheckDistance);
    }
}
