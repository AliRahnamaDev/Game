using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    #region Def

     [Header("          *********Movement*********")]
     public float speed = 5f;
     public bool isChangeFacingActive = true;
     public bool isFacingRight = true;
     public int faceDirection = 1;
     public bool isRunning = false;
     public bool isGrounded = true;
     public bool isWallDitected;
     
     [Header("          *********Jump*********")]
     public float jumpForce = 6f;
     public bool isWallJumping;
     public Vector2 wallJumpForce;
     public float wallJumpingDuration;
     public int jumpcount = 2;
     public bool canDoublejump = true;
     
     [Header("          *********Knock*********")]
     public bool isKnocked;
     private float knockbackDuration = 0.65f;
     public Vector2 knockbackPower;
     public bool canBeKnocked = true;
     
    [Header("          *********Others*********")]
     public float slidingSpeed = 2f;
     public LayerMask whatIsground;
     public LayerMask whatIstrap;
     public float wallCheckDistance = 0.1f;
     public float groundCheckDistace = 0.1f;
     public GameObject DeadVfx;
     public bool canBeControlled =false;
     private float defaultGravityScale;
     public Rigidbody2D _rb;
     private Animator _anim;
     private BoxCollider2D _bc;
    
    #endregion
    
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _bc = GetComponent<BoxCollider2D>();
        defaultGravityScale = 4.5f;
        RespawnFinished(false);
    }
    void Update()
    {
        WallDetected();
        GroundDetect();
        CanDoubleJump();
        WallSliding();
        WallSlidingAnimation();
        WallJump();JumpingAnimation();
        if(!canBeControlled) return;
        if (!isKnocked) MoveX();
        Jump();
        FaceDirectionBaseonInt();
        ChangeFacing();
        RunningAnimation();
        SetWallJumpingOff();
        DoubleJumpAfterWallJumping();
        WallSlidingFaster();
        
    }

    public void push(float Power)
    {
        _rb.velocity = new Vector2(transform.position.x, Power);
    }
    
    
    public void MakeRespawnFinishedtrue()
    {
        RespawnFinished(true);
    }
    public void RespawnFinished(bool finished)
    {
        
        if (finished)
        {
            _rb.gravityScale = defaultGravityScale;
            canBeControlled = true;
            _bc.enabled = true;
        }
        else
        {
            canBeControlled = false;
            _rb.gravityScale = 0;
            _bc.enabled = false;
        }
    }
    public void Die()
    {
        Destroy(gameObject);
        GameObject newFx = Instantiate(DeadVfx, transform.position, Quaternion.identity);
        Destroy(newFx, 0.3f);
    }

    private void GroundDetect()
    {
        bool groundedOnTrap = Physics2D.Raycast(new Vector2(transform.position.x - 0.55f * faceDirection, transform.position.y), Vector2.down, groundCheckDistace, whatIstrap);
        bool groundedOnGround = Physics2D.Raycast(new Vector2(transform.position.x - 0.55f * faceDirection, transform.position.y), Vector2.down, groundCheckDistace, whatIsground);
    
        isGrounded = groundedOnTrap || groundedOnGround;
    }

    private void WallDetected()
    {
        isWallDitected = Physics2D.Raycast(transform.position, new Vector2(1, 0) * faceDirection, wallCheckDistance, whatIsground);
 
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position,new Vector2(transform.position.x+wallCheckDistance*faceDirection,transform.position.y) );
        Gizmos.DrawLine(new Vector2(transform.position.x - (float)0.55 * faceDirection, transform.position.y), new Vector2(transform.position.x - (float)0.55 * faceDirection, transform.position.y-groundCheckDistace));
    }

    private void CanDoubleJump()
    {
        if (isGrounded || isWallDitected)
            canDoublejump = true;
    }
    private void JumpingAnimation()
    {
        if (_rb.velocity.y != 0 && !isGrounded)
            _anim.SetBool("isJumping", true);
        else
            _anim.SetBool("isJumping", false);
        
    }

    private void RunningAnimation()
    {
        IsPlayerRunning();
        _anim.SetBool("isRunning", isRunning);
    }

    private void IsPlayerRunning()
    {

        if (_rb.velocity.x != 0 && isGrounded && !isWallDitected)
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isWallDitected)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
            _anim.SetBool("isJumping", true);
        }
        else if (Input.GetKeyDown(KeyCode.Space) && !isGrounded&&canDoublejump)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
            _anim.SetBool("isJumping", true);
            canDoublejump = false;
        }

    }

    public void MoveX()
    {
        if ((isWallDitected && !isGrounded) || isWallJumping)
        {
            return;
        }
        float moveInput = Input.GetAxis("Horizontal");
        _rb.velocity = new Vector2(moveInput * speed, _rb.velocity.y);
    }

    public void KnockBack(float sourceDamageXposition)
    {
        float KnockBackdir = 1;
        if (transform.position.x < sourceDamageXposition)
        {
            KnockBackdir = -1;
        }
        
        if (isKnocked)
            return;
        StartCoroutine(KnockBackRoutin());
        _anim.SetTrigger("Hit");
        _rb.velocity = new Vector2(knockbackPower.x * KnockBackdir, knockbackPower.y);
    }

    IEnumerator KnockBackRoutin()
    {
        canBeKnocked = false;
        isKnocked = true;
        yield return new WaitForSeconds(knockbackDuration);
        canBeKnocked = true;
        isKnocked = false;
    }
    private void DoubleJumpAfterWallJumping()
    {
        if (isWallJumping && !isWallDitected && Input.GetKeyDown(KeyCode.Space))
        {
            isWallJumping = false;
        }
    }
    private void SetWallJumpingOff()
    {
        if (isGrounded)
        {
            isWallJumping = false;
        }
    }

    private void WallSlidingFaster()
    {
        if (isWallDitected && Input.GetKey(KeyCode.S))
        {
            _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y * 3f);
        }
    }

    private void WallJump()
    {
        if (isWallDitected && Input.GetKeyDown(KeyCode.Space) && !isGrounded)
        {
            Flip();
            StartCoroutine(WallJumpingTime());
            _rb.velocity = new Vector2(wallJumpForce.x *(float)0.1* -faceDirection, wallJumpForce.y*(float)0.1);
            jumpcount--;
        }
        if (isWallDitected && Input.GetKeyDown(KeyCode.Space) && (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A)) && !isGrounded)
        {
            Flip();
            StartCoroutine(WallJumpingTime());
            _rb.velocity = new Vector2(wallJumpForce.x * -faceDirection, wallJumpForce.y);
            jumpcount--;
        }
        if (isWallDitected && Input.GetKeyDown(KeyCode.Space) && Input.GetKey(KeyCode.W)&&!isGrounded)
        {
            Flip();
            StartCoroutine(WallJumpingTime());
            _rb.velocity = new Vector2(wallJumpForce.x*(float)0.3 * -faceDirection, wallJumpForce.y*(float)1.2);
            jumpcount--;
        }
    }

    IEnumerator WallJumpingTime()
    {
        isWallJumping = true;
        yield return new WaitForSeconds(wallJumpingDuration);
        isWallJumping = false;
    }

    private void WallSlidingAnimation()
    {
        _anim.SetBool("IsWallSliding", isWallDitected);
    }

    private void WallSliding()
    {
        if (isWallDitected && !isGrounded && !isWallJumping)
        {
            isChangeFacingActive = false;
            _rb.velocity = new Vector2(_rb.velocity.x, -slidingSpeed);
            jumpcount = 2;
        }
        else
        {
            isChangeFacingActive = true;
        }
    }

    private void FaceDirectionBaseonInt()
    {
        if (isFacingRight)
        {
            faceDirection = 1;
        }
        else
        {
            faceDirection = -1;
        }
    }

    public void Flip()
    {
        transform.Rotate(0, 180, 0);
        isFacingRight = !isFacingRight;
    }

    void ChangeFacing()
    {
        if (!isChangeFacingActive)
            return;
        if (Input.GetKey(KeyCode.A) && isFacingRight)
        {
            Flip();
        }
        else if (Input.GetKey(KeyCode.D) && !isFacingRight)
        {
            Flip();
        }
    }
}