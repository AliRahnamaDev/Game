using System.Collections;
using UnityEngine;

public class Enemy_Cow : MonoBehaviour
{
    
    [Header("Flip Delay")]
    public float backFlipDelay = 1.5f;
    private bool flipPending = false;
    private float flipTimer = 0f;
    private bool isPlayerBehind = false;

    [Header("Status")]
    public bool isAttacking = false;
    public bool isWalking = false;
    public bool playerVisible = false;
    
    [Header("Move Settings")]
    public float normalSpeed = 2f;
    public float chaseSpeed = 5f;
    public float waitTime = 2f;
    private float currentSpeed;

    [Header("Detection Settings")]
    public Vector2 detectionBoxSize = new Vector2(3f, 1f);
    public Vector2 detectionBoxOffset = new Vector2(1.5f, 0f);
    public LayerMask whatIsPlayer;
    public LayerMask whatIsGround;

    [Header("Attack Zone Settings")]
    public Vector2 attackBoxSize = new Vector2(1f, 1f);
    public Vector2 attackBoxOffset = new Vector2(1f, 0f);
    public GameObject hitbox;
    
    [Tooltip("حداقل و حداکثر تأخیر فعال شدن hitbox")]
    public float minAttackDelay = 0.5f;
    public float maxAttackDelay = 2f;

    
    [Header("Attack Control")]
    public float attackPauseDuration = 1f;
    private bool isAttackCooldown = false;

    [Header("Check Points")]
    public Transform wallCheck;
    public float wallCheckDistance = 0.2f;
    public Transform groundCheck;
    public float groundCheckDistance = 0.2f;

    private Rigidbody2D rb;
    private bool isFacingRight = true;
    private bool isWaiting = false;
    private bool isAttackingRange = false;
    private Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentSpeed = normalSpeed;
    }

    void Update()
    {
        SetStatus();
        Animate();
        DetectPlayer();
        TryActivateHitbox();

        if (flipPending && isPlayerBehind)
        {
            flipTimer += Time.deltaTime;
            if (flipTimer >= backFlipDelay)
            {
                Flip();
                flipPending = false;
            }
        }

        if (!isWaiting && !isAttackingRange)
            Patrol();
    }


    void Patrol()
    {
        Vector2 direction = isFacingRight ? Vector2.right : Vector2.left;
        rb.velocity = new Vector2(direction.x * currentSpeed, rb.velocity.y);

        bool hitWall = Physics2D.Raycast(wallCheck.position, direction, wallCheckDistance, whatIsGround);
        bool noGround = !Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);

        if (hitWall || noGround)
        {
            StartCoroutine(WaitAndFlip());
        }
    }

    IEnumerator WaitAndFlip()
    {
        isWaiting = true;
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(waitTime);
        Flip();
        isWaiting = false;
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    void DetectPlayer()
    {
        Vector2 offset = new Vector2(
            isFacingRight ? detectionBoxOffset.x : -detectionBoxOffset.x,
            detectionBoxOffset.y
        );
        Vector2 boxCenter = (Vector2)transform.position + offset;
        Collider2D hit = Physics2D.OverlapBox(boxCenter, detectionBoxSize, 0, whatIsPlayer);

        if (hit != null)
        {
            Vector2 dirToPlayer = hit.transform.position - transform.position;
            RaycastHit2D ray = Physics2D.Raycast(transform.position, dirToPlayer.normalized, dirToPlayer.magnitude, whatIsGround);

            if (ray.collider == null)
            {
                // 🔁 بررسی اینکه پلیر پشت سر است
                bool playerIsBehind = (isFacingRight && hit.transform.position.x < transform.position.x) ||
                                      (!isFacingRight && hit.transform.position.x > transform.position.x);

                if (playerIsBehind)
                {
                    if (!flipPending)
                    {
                        flipPending = true;
                        flipTimer = 0f;
                    }

                    isPlayerBehind = true;
                }
                else
                {
                    flipPending = false;
                    isPlayerBehind = false;
                }

                currentSpeed = chaseSpeed;
                return;
            }
        }

        // ریست اگر پلیر دیده نشد
        currentSpeed = normalSpeed;
        flipPending = false;
        isPlayerBehind = false;
    }


    void TryActivateHitbox()
    {
        if (isAttackCooldown) return;

        Vector2 center = (Vector2)transform.position +
                         (isFacingRight ? attackBoxOffset : new Vector2(-attackBoxOffset.x, attackBoxOffset.y));

        Collider2D player = Physics2D.OverlapBox(center, attackBoxSize, 0, whatIsPlayer);
        playerVisible = false;

        if (player != null)
        {
            Vector2 dirToPlayer = player.transform.position - transform.position;
            RaycastHit2D ray = Physics2D.Raycast(transform.position, dirToPlayer.normalized, dirToPlayer.magnitude, whatIsGround);
            if (ray.collider == null)
            {
                playerVisible = true;
            }
        }

        isAttackingRange = playerVisible;

        if (playerVisible && !isAttackCooldown)
        {
            rb.velocity = Vector2.zero;
            StartCoroutine(AttackPause());
        }
    }



    IEnumerator AttackPause()
    {
        isAttackCooldown = true;
        rb.velocity = Vector2.zero;

        // تاخیر تصادفی قبل از حمله
        float delay = Random.Range(minAttackDelay, maxAttackDelay);
        yield return new WaitForSeconds(delay);

        // حالا حمله شروع شود
        isAttacking = true;
        hitbox.SetActive(true);

        // مدت زمان نمایش hitbox
        yield return new WaitForSeconds(attackPauseDuration);

        hitbox.SetActive(false);
        isAttacking = false;
        isAttackCooldown = false;
    }



    public void SetStatus()
    {
        if(Mathf.Abs(rb.velocity.x) > 0.3) isWalking = true;
        else isWalking = false;
    }

    public void Animate()
    {
        anim.SetBool("isAttacking",isAttacking);
        anim.SetBool("isWalking",isWalking);
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 boxOffset = new Vector2(
            isFacingRight ? detectionBoxOffset.x : -detectionBoxOffset.x,
            detectionBoxOffset.y
        );
        Vector2 boxCenter = (Vector2)transform.position + boxOffset;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCenter, detectionBoxSize);

        Vector2 atkOffset = new Vector2(
            isFacingRight ? attackBoxOffset.x : -attackBoxOffset.x,
            attackBoxOffset.y
        );
        Vector2 atkCenter = (Vector2)transform.position + atkOffset;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(atkCenter, attackBoxSize);

        if (wallCheck != null)
        {
            Gizmos.color = Color.blue;
            Vector2 dir = isFacingRight ? Vector2.right : Vector2.left;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + (Vector3)(dir * wallCheckDistance));
        }

        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);
        }
    }
}
