using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordPlayer : MonoBehaviour,IKnockbackable,IBlockable
{
    private Animator animator;
    private Rigidbody2D rb;

    public bool isBlocking = false;
    public float blockDamageMultiplier = 0.4f;
    public float blockKnockbackMultiplier = 0.3f;
    
    public bool canMoveHorizontally = true;
    
    [Header("Input Settings")]
    [SerializeField] private KeyCode moveRightKey = KeyCode.D;
    [SerializeField] private KeyCode moveLeftKey = KeyCode.A;
    [SerializeField] private KeyCode runKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode attackKey = KeyCode.Mouse0;
    [SerializeField] private KeyCode blockKey = KeyCode.Mouse1;


    [SerializeField] private LayerMask whatIsEnemy;
    [SerializeField] private float EnemyCheckRadius;
    [SerializeField] private Transform enemyCheck;

    [Header("AttackDetails")]
    [SerializeField] private GameObject AttackSource;
    [SerializeField] private float attackDuration = 0.2f;

    [Header("Movement")]
    public float jumpForceAfterhitEnemy = 5;
    public float walkSpeed = 2.5f;
    public float runSpeed = 6f;
    public float jumpForce = 10f;
    public float WallslideSpeed = 1.5f;
    private float defualtGravity;
    public float Jumpmultiplier = 1f;

    [Header("WallJumpForce")]
    public float WallJumpxForce = 2;
    public float WallJumpyForce = 2;

    [Header("Status")]
    public bool canFlip = true;
    public bool isGrounded;
    public bool IsWallDitected;
    public bool isJumping;
    public bool isRunning;
    public bool isWalking;
    public bool isFalling;
    public bool isFacingRight = true;
    public float facingDirection = 1;
    public bool isAttacking;

    [Header("Layers")]
    public float wallCheckDistance;
    public float groundCheckDistance;
    public LayerMask whatIsGround;

    private bool isWallJumping = false;
    [SerializeField] private float wallJumpControlLockTime = 0.2f;

    public void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        defualtGravity = rb.gravityScale;
    }

    public void Update()
    {
        isBlocking = Input.GetKey(blockKey) && !isAttacking;

        if(!canMoveHorizontally) 
            return;
        if (Input.GetKey(attackKey))
        {
            Attack();
        }

        WallJump();
        HandleWallSlide();
        HandleEnemyCollision();
        WallDetected();
        DetectGround();
        HandleJump();
        setStatus();
        Animate();
        HandleFlip();
        Movement();
    }

    
    public void PauseHorizontalMovement(float duration)
    {
        StartCoroutine(PauseX(duration));
    }

    private IEnumerator PauseX(float time)
    {
        canMoveHorizontally = false;
        yield return new WaitForSeconds(time);
        canMoveHorizontally = true;
    }
    public void WallJump()
    {
        if (IsWallDitected && !isGrounded && Input.GetKeyDown(jumpKey) && !isWallJumping)
        {
            isWallJumping = true;

            rb.gravityScale = defualtGravity;
            rb.velocity = Vector2.zero;

            float jumpDirection = isFacingRight ? -1 : 1;
            rb.velocity = new Vector2(WallJumpxForce * jumpDirection, WallJumpyForce);
            StartCoroutine(Flip());
            StartCoroutine(ResetWallJumping());
        }
    }

    IEnumerator ResetWallJumping()
    {
        yield return new WaitForSeconds(wallJumpControlLockTime);
        isWallJumping = false;
    }

    public void HandleWallSlide()
    {
        if (IsWallDitected && !isGrounded && !isWallJumping)
        {
            //isFalling = false;
            rb.gravityScale = 0;
            rb.velocity = new Vector2(rb.velocity.x, -WallslideSpeed);
        }
        else
        {
            rb.gravityScale = defualtGravity;
        }
    }

    public void Attack()
    {
        if (isAttacking || isBlocking) return;

        StartCoroutine(ActivateAttackSource());
    }


    IEnumerator ActivateAttackSource()
    {
        AttackSource.SetActive(true);
        yield return new WaitForSeconds(attackDuration);
        AttackSource.SetActive(false);
    }

    public void Movement()
    {
        if(!canMoveHorizontally) return;
        if (isWallJumping) return;
        if (IsWallDitected && !isGrounded) return;

        float moveInput = 0f;
        if (Input.GetKey(moveRightKey)) moveInput = 1f;
        else if (Input.GetKey(moveLeftKey)) moveInput = -1f;

        float speed = Input.GetKey(runKey) ? runSpeed : walkSpeed;
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        if (isGrounded && IsWallDitected) rb.velocity = new Vector2(0f, rb.velocity.y);
    }

    public void Jump()
    {
        if (Mathf.Abs(rb.velocity.x) > 2f)
            rb.velocity = new Vector2(rb.velocity.x, jumpForce * Jumpmultiplier);
        else
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    public void HandleJump()
    {
        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            Jump();
        }
    }

    public void setStatus()
    {
        isWalking = Mathf.Abs(rb.velocity.x) > 0.3f && Mathf.Abs(rb.velocity.x)<runSpeed-0.1f;
        isRunning = Mathf.Abs(rb.velocity.x) >= runSpeed-0.05f;
        isJumping = rb.velocity.y > 0.2f;
        if (IsWallDitected)
        {
            isFalling = false;
        }
        else if (rb.velocity.y < -0.3f)
        {
            isFalling = true;
        }
        else
        {
            isFalling = false;
        }
        if (isJumping || isFalling)
        {
            isWalking = false;
            isRunning = false;
        }

        isAttacking = Input.GetKey(attackKey);
    }
    
    public bool IsBlocking() => isBlocking;
    public float GetDamageReductionMultiplier() => isBlocking ? blockDamageMultiplier : 1f;
    public float GetKnockbackReductionMultiplier() => isBlocking ? blockKnockbackMultiplier : 1f;

    public void Animate()
    {
        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isJumping", isJumping);
        animator.SetBool("isFalling", isFalling);
        animator.SetBool("isAttacking", isAttacking);
        animator.SetBool("isWallSliding", IsWallDitected);
        animator.SetBool("isBlocking", isBlocking);
    }

    public void HandleFlip()
    {
        if (isWallJumping) return;

        if (Input.GetKey(moveRightKey) && !isFacingRight)
            StartCoroutine(Flip());
        else if (Input.GetKey(moveLeftKey) && isFacingRight)
            StartCoroutine(Flip());
    }

    private void HandleEnemyCollision()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(enemyCheck.position, EnemyCheckRadius, whatIsEnemy);

        foreach (var enemy in enemies)
        {
            Enemy newEnemy = enemy.GetComponent<Enemy>();
            if (newEnemy != null)
            {
                if (rb.velocity.y < -0.01 && transform.position.y > newEnemy.transform.position.y + 0.4f)
                {
                    rb.velocity = new Vector2(rb.velocity.x, 0);
                    rb.AddForce(Vector2.up * jumpForceAfterhitEnemy, ForceMode2D.Impulse);
                    newEnemy.Die();
                }
            }
        }
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

    private void WallDetected()
    {
        IsWallDitected = Physics2D.Raycast(transform.position, new Vector2(1, 0) * facingDirection, wallCheckDistance, whatIsGround);
    }

    public void DetectGround()
    {
        Vector2 origin = new Vector2(transform.position.x - 0.5f, transform.position.y - 0.5f);
        Vector2 origin2 = new Vector2(transform.position.x + 0.5f, transform.position.y - 0.5f);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, whatIsGround);
        RaycastHit2D hit2 = Physics2D.Raycast(origin2, Vector2.down, groundCheckDistance, whatIsGround);
        isGrounded = hit.collider != null || hit2.collider != null;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + wallCheckDistance * facingDirection, transform.position.y));
        Gizmos.color = Color.red;
        Vector2 origin = new Vector2(transform.position.x - 0.5f, transform.position.y - 0.5f);
        Vector2 origin2 = new Vector2(transform.position.x + 0.5f, transform.position.y - 0.5f);
        Gizmos.DrawLine(origin, origin + Vector2.down * groundCheckDistance);
        Gizmos.DrawLine(origin2, origin2 + Vector2.down * groundCheckDistance);
        Gizmos.DrawWireSphere(enemyCheck.position, EnemyCheckRadius);
    }
}
