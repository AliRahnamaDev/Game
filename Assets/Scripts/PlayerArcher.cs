using System.Collections;
using UnityEngine;

public class PlayerArcher : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    [SerializeField] private LayerMask whatIsEnemy;
    [SerializeField] private float EnemyCheckRadius;
    [SerializeField] private Transform enemyCheck;
    
    [Header("AttackDetails")]
    [SerializeField] private GameObject AttackSource;
    [SerializeField] private float attackDuration = 0.2f;
    
    [Header("Movement")]
    public float jumpForceAfterhitEnemy = 5;
    public float SlowSpeed = 5;
    public float boostSpeed = 7.5f;
    public float jumpForce = 14;
    public float speedrunjump;
    public bool isFacingRight = true;
    public float FacingDirection = 1;
    public float normalspeedtoland=-2;
    public float minimomspeedtoactivespeedrun;
    
    [Space(2)]
    [Header("Jump Settings")]
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;
    public bool isGrounded;
    
    [Space(2)]
    [Header("Staus")]
    public bool isJumping = false;
    public bool isWalking = false;
    public bool isRunning = false;
    public bool isAttacking = false;
    public bool isShoting = false;
    public bool ShouldLand = false;
    
    [Space(2)]
    [Header("Shooting Settings")]
    public GameObject arrowPrefab;
    public Transform arrowSpawnPoint;
    public float arrowSpeed = 10f;
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;


    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleEnemyCollision();
        DetectGround();
        HandleJump();  
        SetStatus();      
        HandleMovement();
        HandleFlip();
        Animate();
        Attack();
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
        //Debug.Log("Attack started!"); // برای تست
        AttackSource.SetActive(true);
        yield return new WaitForSeconds(attackDuration);
        AttackSource.SetActive(false);
        //Debug.Log("Attack ended!");
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
    private void ShootArrow()
    {
        if (Time.time < nextFireTime) return;

        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.identity);

        // تعیین جهت تیر
        float direction = isFacingRight ? 1f : -1f;
        arrow.GetComponent<Rigidbody2D>().velocity = new Vector2(direction * arrowSpeed, 0f);

        // اگر تیر Sprite یا FlipX داره:
        if (!isFacingRight)
        {
            Vector3 scale = arrow.transform.localScale;
            scale.x *= -1;
            arrow.transform.localScale = scale;
        }

        nextFireTime = Time.time + fireRate;
    }


    public void HandleMovement()
    {
        float moveInput = 0f;

        if (Input.GetKey(KeyCode.D))
            moveInput = 1f;
        else if (Input.GetKey(KeyCode.A))
            moveInput = -1f;

        float speed = Input.GetKey(KeyCode.LeftShift) ? boostSpeed : SlowSpeed;
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
    }

    public void HandleFlip()
    {
        if (Input.GetKey(KeyCode.D) && !isFacingRight)
            Flip();
        else if (Input.GetKey(KeyCode.A) && isFacingRight)
            Flip();
    }

    public void Flip()
    {
        isFacingRight = !isFacingRight;
        FacingDirection = isFacingRight ? 1 : -1;
        transform.Rotate(0f, 180f, 0f);
    }

    public void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isRunning)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        else if (Input.GetKeyDown(KeyCode.Space) && isGrounded && minimomspeedtoactivespeedrun<Mathf.Abs(rb.velocity.x))
        {
            rb.AddForce(Vector2.up * speedrunjump, ForceMode2D.Impulse);
        }
        
    }

    public void DetectGround()
    {
        Vector2 origin = new Vector2((transform.position.x - 0.5f), transform.position.y - 0.5f);
        Vector2 origin2 = new Vector2((transform.position.x + 0.5f), transform.position.y - 0.5f);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, groundLayer);
        RaycastHit2D hit2 = Physics2D.Raycast(origin2, Vector2.down, groundCheckDistance, groundLayer);
        isGrounded = hit.collider != null || hit2.collider != null;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 origin = new Vector2((transform.position.x - 0.5f), transform.position.y - 0.5f);
        Vector2 origin2 = new Vector2((transform.position.x + 0.5f), transform.position.y - 0.5f);
        Gizmos.DrawLine(origin, origin + Vector2.down * groundCheckDistance);
        Gizmos.DrawLine(origin2, origin2 + Vector2.down * groundCheckDistance);
        Gizmos.DrawWireSphere(enemyCheck.position,EnemyCheckRadius);
    }

    public void SetStatus()
    {
        if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) && isGrounded)
        {
            isWalking = true;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                isWalking = false;
                isRunning = true;
            }
            else
            {
                isRunning = false;
            }
        }
        else
        {
            isWalking = false;
            isRunning = false;
        }
        
        isJumping = !isGrounded;
        
        if (Input.GetMouseButton(1) && isGrounded && !isRunning && !isWalking)
        {
            ShootArrow();
            isShoting = true;
        }
        else
        {
            isShoting = false;
        }
        if (Input.GetMouseButton(0))
        {
            isAttacking = true;
        }
        else
        {
            isAttacking = false;
        }
        if (rb.velocity.y < normalspeedtoland)
        {
            ShouldLand = true;
        }
        else
        {
            ShouldLand = false;
        }
        if (isJumping)
        {
            isWalking = false;
            isRunning = false;
        }
    }

    
    public void Animate()
    {
        animator.SetBool("isShoting", isShoting);
        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isJumping", isJumping);
        animator.SetBool("canLand", ShouldLand);
        animator.SetBool("isAttacking", isAttacking);
    }
}
