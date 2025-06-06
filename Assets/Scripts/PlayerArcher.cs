using System.Collections;
using UnityEngine;

public class PlayerArcher : MonoBehaviour,IKnockbackable
{
    private Animator animator;
    private Rigidbody2D rb;

    public bool canMoveHorizontally;
    
    [Header("Input Settings")]
    [SerializeField] private KeyCode moveRightKey = KeyCode.D;
    [SerializeField] private KeyCode moveLeftKey = KeyCode.A;
    [SerializeField] private KeyCode runKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode shootKey = KeyCode.Mouse1;
    [SerializeField] private KeyCode attackKey = KeyCode.Mouse0;

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
    public float normalspeedtoland = -2;
    public float minimomspeedtoactivespeedrun;

    [Space(2)]
    [Header("Jump Settings")]
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;
    public bool isGrounded;

    [Space(2)]
    [Header("Status")]
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
        if (isAttacking)
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

    private void ShootArrow()
    {
        if (Time.time < nextFireTime) return;

        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.identity);

        float direction = isFacingRight ? 1f : -1f;
        arrow.GetComponent<Rigidbody2D>().velocity = new Vector2(direction * arrowSpeed, 0f);

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
        if(!canMoveHorizontally)
            return;
        float moveInput = 0f;

        if (Input.GetKey(moveRightKey))
            moveInput = 1f;
        else if (Input.GetKey(moveLeftKey))
            moveInput = -1f;

        float speed = Input.GetKey(runKey) ? boostSpeed : SlowSpeed;
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
    }

    public void HandleFlip()
    {
        if (Input.GetKey(moveRightKey) && !isFacingRight)
            Flip();
        else if (Input.GetKey(moveLeftKey) && isFacingRight)
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
        if (Input.GetKeyDown(jumpKey) && isGrounded && !isRunning)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        else if (Input.GetKeyDown(jumpKey) && isGrounded && minimomspeedtoactivespeedrun < Mathf.Abs(rb.velocity.x))
        {
            rb.AddForce(Vector2.up * speedrunjump, ForceMode2D.Impulse);
        }
    }

    public void DetectGround()
    {
        Vector2 origin = new Vector2(transform.position.x - 0.5f, transform.position.y - 0.5f);
        Vector2 origin2 = new Vector2(transform.position.x + 0.5f, transform.position.y - 0.5f);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, groundLayer);
        RaycastHit2D hit2 = Physics2D.Raycast(origin2, Vector2.down, groundCheckDistance, groundLayer);
        isGrounded = hit.collider != null || hit2.collider != null;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 origin = new Vector2(transform.position.x - 0.5f, transform.position.y - 0.5f);
        Vector2 origin2 = new Vector2(transform.position.x + 0.5f, transform.position.y - 0.5f);
        Gizmos.DrawLine(origin, origin + Vector2.down * groundCheckDistance);
        Gizmos.DrawLine(origin2, origin2 + Vector2.down * groundCheckDistance);
        Gizmos.DrawWireSphere(enemyCheck.position, EnemyCheckRadius);
    }

    public void SetStatus()
    {
        if ((Input.GetKey(moveLeftKey) || Input.GetKey(moveRightKey)) && isGrounded)
        {
            isWalking = true;
            if (Input.GetKey(runKey))
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

        if (Input.GetKey(shootKey) && isGrounded && !isRunning && !isWalking)
        {
            ShootArrow();
            isShoting = true;
        }
        else
        {
            isShoting = false;
        }

        if (Input.GetKey(attackKey))
        {
            isAttacking = true;
        }
        else
        {
            isAttacking = false;
        }

        ShouldLand = rb.velocity.y < normalspeedtoland;
        
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
}
