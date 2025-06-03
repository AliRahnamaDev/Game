using System.Collections;
using UnityEngine;

public class SwordEnemy : MonoBehaviour
{
    public float oldspeed=3;
    public float speed = 3;
    public float boostspeed = 6;
    private Rigidbody2D rb;
    private Animator animator;
    private Player player;
    public GameObject Sword;
    
    public int facingDirection = 1;
    public bool facingRight = true;
    public bool isGroundInfrontDetected = true;
    public bool isWallDetected = true;
    public bool canFlip = true;

    [Header("AttackRange")] 
    public float xRange=3;
    public float yRange=3;
    public float AttackDuration = 1.5f;
    
    [Header("Timers")]
    public float timeToCowBack = 2;
    public float Idletime = 2;

    [Header("Collision Settings")]
    [SerializeField] protected float groundCheckDistance;
    [SerializeField] protected float wallCheckDistance;
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] protected Transform groundCheck;

    [Header("Player Detection Box")]
    [SerializeField] private Vector2 detectionBoxSize = new Vector2(2f, 2f);
    [SerializeField] private Vector2 detectionBoxOffset = new Vector2(1f, 0f);
    [SerializeField] private LayerMask whatIsPlayer;
    public bool isPlayerDetected = false;
    
    [Header("Attack Settings")]
    [SerializeField] private float attackRangeX = 2f;
    [SerializeField] private float attackRangeY = 1.5f;
    private bool isAttacking = false;

    public bool canWalk = true;
    public bool canAttack = true;
    void Start()
    {
        Sword.SetActive(false);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Attack();
        HandleMovement();
        AnimateCow();
        HandleCollisions();
        DetectPlayerInBox();
        HandleIdle();
        float xValue = player.transform.position.x;
        float yValue = player.transform.position.y;
        HandleFlip(xValue, yValue);
        speedChanger();
    }

    public void speedChanger()
    {
        if (isPlayerDetected)
        {
            speed = boostspeed;
        }
        else
        {
            speed = oldspeed;
        }
    }

    public void Attack()
    {
        float Xdistance = Mathf.Abs(player.transform.position.x - transform.position.x);
        float Ydistance = Mathf.Abs(player.transform.position.y - transform.position.y);
        if (Xdistance < xRange && Ydistance < yRange)
        {
            StartCoroutine(HandleAttack());
        }
    }

    IEnumerator HandleAttack()
    {
        if (canAttack)
        {
            canAttack = false;
            canWalk = false;
            animator.SetBool("isAttacking", true);
            rb.velocity = new Vector2(facingDirection * speed / 10, rb.velocity.y);
            yield return new WaitForSeconds(AttackDuration);
            StartCoroutine(HandleSwordActivation());
            animator.SetBool("isAttacking", false);
            rb.velocity = new Vector2(facingDirection * speed, rb.velocity.y);
            canWalk = true;
            canAttack = true;
        }
    }

    IEnumerator HandleSwordActivation()
    {
        Sword.SetActive(true);
        yield return new WaitForSeconds(0.01f);
        Sword.SetActive(false);
    }

    public void HandleMovement()
    {
        if (!canWalk)
            return;

        if (isGroundInfrontDetected && !isWallDetected)
        {
            rb.velocity = new Vector2(facingDirection * speed, rb.velocity.y);
        }
        else
        {
            Flip();
        }
    }

    protected void Flip()
    {
        canFlip = true;
        facingDirection *= -1;
        transform.Rotate(0f, 180f, 0f);
        facingRight = !facingRight;
    }

    public void HandleFlip(float xValue, float yValue)
    {
        if (((xValue > transform.position.x && !facingRight) || (xValue < transform.position.x && facingRight)) &&
           Mathf.Abs(yValue - transform.position.y) < 3)
        {
            if (canFlip)
            {
                canFlip = false;
                Invoke(nameof(Flip), timeToCowBack);
            }
        }
    }

    public void HandleCollisions()
    {
        isGroundInfrontDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        isWallDetected = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - 1.5f), Vector2.right * facingDirection, wallCheckDistance, whatIsGround);
    }

    public void HandleIdle()
    {
        if (!isGroundInfrontDetected || isWallDetected)
        {
            StartCoroutine(Idle());
        }
    }

    private void AnimateCow()
    {
        animator.SetBool("isRunning", rb.velocity.x != 0);
    }

    private void DetectPlayerInBox()
    {
        Vector2 boxCenter = (Vector2)transform.position + new Vector2(facingDirection * detectionBoxOffset.x, detectionBoxOffset.y);
        Collider2D hit = Physics2D.OverlapBox(boxCenter, detectionBoxSize, 0f, whatIsPlayer);
        if (hit != null)
        {
            isPlayerDetected = true;
        }
        else
        {
            isPlayerDetected = false;
        }
    }
    
    
    IEnumerator Idle()
    {
        canWalk = false;
        rb.velocity = Vector2.zero;
        animator.SetBool("isRunning", false);
        animator.SetBool("isAttacking", false);
        yield return new WaitForSeconds(Idletime);
        canWalk = true;
        animator.SetBool("isRunning", true);
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));

        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(transform.position.x, transform.position.y - 1.5f, transform.position.z),
                        new Vector2(transform.position.x + (wallCheckDistance * facingDirection), transform.position.y - 1.5f));

        Gizmos.color = Color.yellow;
        Vector2 boxCenter = Application.isPlaying
            ? (Vector2)transform.position + new Vector2(facingDirection * detectionBoxOffset.x, detectionBoxOffset.y)
            : (Vector2)transform.position + new Vector2(1f * detectionBoxOffset.x, detectionBoxOffset.y);
        Gizmos.DrawWireCube(boxCenter, detectionBoxSize);
    }
}
