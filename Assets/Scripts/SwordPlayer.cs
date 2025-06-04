using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordPlayer : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;

    [SerializeField] private LayerMask whatIsEnemy;
    [SerializeField] private float EnemyCheckRadius;
    [SerializeField] private Transform enemyCheck;

    [Space(2)]
    [Header("AttackDetails")] [SerializeField]
    private GameObject AttackSource;
    [SerializeField] private float attackDuration = 0.2f;

    [Space(2)]
    [Header("Movement")] public float jumpForceAfterhitEnemy = 5;
    public float walkSpeed = 2.5f;
    public float runSpeed = 6f;
    public float jumpForce = 10f;
    public float WallslideSpeed = 1.5f;
    private float defualtGravity;
    public float Jumpmultiplier = 1f;
    [Space(2)]
    [Header("WallJumpForce")]
    public float WallJumpxForce=2;
    public float WallJumpyForce=2;
    [Space(2)]
    [Header("Status")] public bool canFlip = true;
    public bool isGrounded;
    public bool IsWallDitected;
    public bool isJumping;
    public bool isRunning;
    public bool isWalking;
    public bool isFalling;
    public bool isFacingRight = true;
    public float facingDirection = 1;
    public bool isAttacking;
    

    [Space(2)]
    [Header("Layers")] public float wallCheckDistance;
    public float groundCheckDistance;
    public LayerMask whatIsGround;
    
    public void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        defualtGravity = rb.gravityScale;
    }

    public void Update()
    {
        
        if (Input.GetMouseButton(0))
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

    private bool isWallJumping = false;
    [SerializeField] private float wallJumpControlLockTime = 0.2f;

    public void WallJump()
    {
        if (IsWallDitected && !isGrounded && Input.GetKeyDown(KeyCode.Space) && !isWallJumping)
        {
            isWallJumping = true;

            rb.gravityScale = defualtGravity;

            // جلوگیری از Wallslide با خاموش‌کردن سرعت رو به پایین
            rb.velocity = Vector2.zero;

            float jumpDirection = isFacingRight ? -1 : 1;

            rb.velocity = new Vector2(WallJumpxForce * jumpDirection, WallJumpyForce);

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
            isFalling = false;
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
        if (isAttacking) // کلیک راست فقط یک‌بار
        {
            StartCoroutine(ActivateAttackSource());
        }
    }

    IEnumerator ActivateAttackSource()
    {
        AttackSource.SetActive(true);
        yield return new WaitForSeconds(attackDuration);
        AttackSource.SetActive(false);
    }

    public void Movement()
    {
        if (isWallJumping) return;

        if(IsWallDitected && !isGrounded)return;
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
        if (Mathf.Abs(rb.velocity.x) > 2f)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce * Jumpmultiplier);
        }
        else
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    public void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    public void setStatus()
    {
        if (Mathf.Abs(rb.velocity.x) > 0 && Mathf.Abs(rb.velocity.x) < runSpeed) isWalking = true;
        else isWalking = false;

        if (Mathf.Abs(rb.velocity.x) >= runSpeed) isRunning = true;
        else isRunning = false;

        if (rb.velocity.y > 0.1) isJumping = true;
        else isJumping = false;
        if (isJumping)
        {
            isRunning = false;
            isWalking = false;
        }

        if (rb.velocity.y < -0.1) isFalling = true;
        else isFalling = false;
        if (isFalling)
        {
            isRunning = false;
            isWalking = false;
        }

        if (Input.GetMouseButton(0)) isAttacking = true;
        else isAttacking = false;
    }

    public void Animate()
    {
        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isJumping", isJumping);
        animator.SetBool("isFalling", isFalling);
        animator.SetBool("isAttacking", isAttacking);
        animator.SetBool("isWallSliding", IsWallDitected);
    }

    public void HandleFlip()
    {
        if (isWallJumping) return;

        if (Input.GetKey(KeyCode.D) && !isFacingRight)
            StartCoroutine(Flip());
        else if (Input.GetKey(KeyCode.A) && isFacingRight)
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
                // فقط اگر پایین‌تر از دشمن باشیم (برای جلوگیری از برخورد از بغل یا بالا)
                if (rb.velocity.y < -0.01 && transform.position.y > newEnemy.transform.position.y + 0.4f)
                {
                    rb.velocity = new Vector2(rb.velocity.x, 0); // حذف سرعت قبلی به بالا
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
        IsWallDitected = Physics2D.Raycast(transform.position, new Vector2(1, 0) * facingDirection, wallCheckDistance,
            whatIsGround);
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
        Gizmos.DrawLine(transform.position,
            new Vector2(transform.position.x + wallCheckDistance * facingDirection, transform.position.y));
        Gizmos.color = Color.red;
        Vector2 origin = new Vector2((transform.position.x - 0.5f), transform.position.y - 0.5f);
        Vector2 origin2 = new Vector2((transform.position.x + 0.5f), transform.position.y - 0.5f);
        Gizmos.DrawLine(origin, origin + Vector2.down * groundCheckDistance);
        Gizmos.DrawLine(origin2, origin2 + Vector2.down * groundCheckDistance);
        Gizmos.DrawWireSphere(enemyCheck.position, EnemyCheckRadius);
    }
}