using UnityEngine;
public class Enemy : MonoBehaviour
{
    [Header("Death details")] [SerializeField]
    private float deathImpact;
    [SerializeField] private float deathRoatationSpeed;
    public bool isDead;
    private int deathDirection = 1;
    [Space]
    [Header("          ***********************")]
    [Space]
    [SerializeField] protected GameObject damageTrigger;
    protected Rigidbody2D rb;
    protected Animator animator;
    protected Collider2D collider;
    
    protected int facingDirection = -1;
    [SerializeField] protected float speed;
    [SerializeField] protected float groundCheckDistance;
    [SerializeField] protected float wallCheckDistance;
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected bool facingRight = false;
    [SerializeField] protected bool isGroundInfrontDetected = true;
    [SerializeField] protected bool isGrounded;
    [SerializeField] protected bool isWallDetected = false;
    [SerializeField] protected float idleTimer;
    [SerializeField] protected float idleDuration;
    
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        collider = GetComponent<Collider2D>();
    }

    protected virtual void Update()
    {
        idleTimer -= Time.deltaTime;
        if (isDead)
        {
            HandleDeath();
        }
    }

    public virtual void Die()
    {
        collider.enabled = false;
        damageTrigger.SetActive(false);
        isDead = true;
        rb.velocity = new Vector2(rb.velocity.x, deathImpact);
        if (Random.Range(0f, 100f) < 50)
        {
            deathDirection *= -1;
        }
    }

    private void HandleDeath()
    {
        Debug.Log("alu");
        transform.Rotate(0,0,(deathRoatationSpeed * (deathDirection)*Time.deltaTime));
    }
    
    private void HandleFlip(float xValue)
    {
        if(xValue > 0  && !facingRight || xValue < 0  && facingRight)
            Flip();
    }

    protected virtual void Flip()
    {
        facingDirection *= -1;
        transform.Rotate(0f, 180f, 0f);
        facingRight = !facingRight;
    }

    protected virtual void HandleCollisions()
    {
        isGrounded=Physics2D.Raycast((transform.position),Vector2.down,groundCheckDistance,whatIsGround);
        isGroundInfrontDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        isWallDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDirection, wallCheckDistance, whatIsGround);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position,new  Vector2(transform.position.x,groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(groundCheck.position,new  Vector2(groundCheck.position.x,groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(transform.position,new  Vector2(transform.position.x + (wallCheckDistance * facingDirection),transform.position.y));
    }
}
