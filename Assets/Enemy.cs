using UnityEngine;
public class Enemy : MonoBehaviour
{
    protected Rigidbody2D rb;
    protected Animator animator;
    
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
    }

    protected virtual void Update()
    {
        idleTimer -= Time.deltaTime;
        
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
