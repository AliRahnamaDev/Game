using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsEnemy;
    [SerializeField] private float EnemyCheckRadius;
    [SerializeField] private Transform enemyCheck;
    #region Def

    [Header("          *********Abilities*********")]
    public bool allowWallSlide = true;
    public bool allowGravityInvert = true;
    public bool allowDash = true;
    [SerializeField] private KeyCode dashKey = KeyCode.LeftShift;

    [Header("          *********Dash Settings*********")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.5f;
    public bool canDash = true;
    public bool isDashing = false;
    public int airDashCount = 2; // تغییر از 1 به 2
    public int currentAirDashes = 0;
    public bool resetAirDashOnGround = true;
    public Color dashTrailColor = Color.white;
    public bool omnidirectionalDash = true;
    public float dashEndVerticalMultiplier = 0.5f;
    public bool canAddDash =false;

    [Header("          *********Gravity*********")]
    public bool isGravityInverted = false;
    public float invertedGravityScale = -4.5f;

    [Header("          *********Movement*********")]
    public float jumpForceAfterhitEnemy = 12;
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

    // [Header("          *********Knock*********")]
    // public bool isKnocked;
    // private float knockbackDuration = 0.65f;
    // public Vector2 knockbackPower;
    // public bool canBeKnocked = true;

    [Header("          *********Others*********")]
    public float slidingSpeed = 2f;
    public LayerMask whatIsground;
    public LayerMask whatIstrap;
    public float wallCheckDistance = 0.1f;
    public float groundCheckDistace = 0.1f;
    public GameObject DeadVfx;
    public bool canBeControlled = false;
    private float defaultGravityScale;
    public Rigidbody2D _rb;
    private Animator _anim;
    private BoxCollider2D _bc;
    private TrailRenderer _trail;
    

    #endregion

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _bc = GetComponent<BoxCollider2D>();
        _trail = GetComponent<TrailRenderer>();
        defaultGravityScale = 4.5f;
        RespawnFinished(false);
        
        if (_trail != null)
        {
            _trail.enabled = false;
            _trail.startColor = dashTrailColor;
            _trail.endColor = new Color(dashTrailColor.r, dashTrailColor.g, dashTrailColor.b, 0);
        }
    }

    void Update()
    {
        HandleEnemyCollision();
        GroundDetect();
        CanDoubleJump();
        
        if (allowWallSlide)
        {
            WallDetected();
            WallSliding();
            WallSlidingAnimation();
            WallJump();
        }
        else
        {
            isWallDitected = false;
            isChangeFacingActive = true;
        }

        JumpingAnimation();

        if (!canBeControlled) return;
        // if (!isKnocked && !isDashing) MoveX();
        if(!isDashing) MoveX();

        if (allowGravityInvert && Input.GetKeyDown(KeyCode.M))
        {
            isGravityInverted = !isGravityInverted;
            ToggleGravity();
        }
        
        if(!isWallDitected ) canAddDash = true;
        if (isWallDitected && canAddDash)
        {
            if(currentAirDashes>-1)
            {
                currentAirDashes--;
            }
            canAddDash = false;
        }
        if (allowDash && Input.GetKeyDown(dashKey))
        {
            TryDash();
        }

        Jump();
        FaceDirectionBaseonInt();
        ChangeFacing();
        RunningAnimation();
        SetWallJumpingOff();
        DoubleJumpAfterWallJumping();
        WallSlidingFaster();
    }

    private void TryDash()
    {
        if (canDash && (isGrounded || currentAirDashes < airDashCount))
        {
            StartCoroutine(Dash());
        }
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
                if (_rb.velocity.y < 0 && transform.position.y > newEnemy.transform.position.y + 0.2f)
                {
                    _rb.velocity = new Vector2(_rb.velocity.x, 0); // حذف سرعت قبلی به بالا
                    _rb.AddForce(Vector2.up * jumpForceAfterhitEnemy, ForceMode2D.Impulse);
                    newEnemy.Die();
                }
            }
        }
    }

    
    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        
        float originalGravity = _rb.gravityScale;
        _rb.gravityScale = 0f;
        
        Vector2 dashDirection = Vector2.zero;
        
        if (omnidirectionalDash)
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");
            
            dashDirection = new Vector2(horizontalInput, verticalInput).normalized;
            
            if (dashDirection == Vector2.zero)
            {
                dashDirection = isFacingRight ? Vector2.right : Vector2.left;
            }
        }
        else
        {
            float dashDirectionX = Input.GetAxisRaw("Horizontal");
            if (dashDirectionX == 0)
            {
                dashDirectionX = faceDirection;
            }
            dashDirection = new Vector2(dashDirectionX, 0f).normalized;
        }
        
        _rb.velocity = dashDirection * dashSpeed;
        
        if (_trail != null)
        {
            _trail.enabled = true;
        }
        yield return new WaitForSeconds(dashDuration);
        
        _rb.gravityScale = originalGravity;
        isDashing = false;
        
        if (!isGrounded)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y * dashEndVerticalMultiplier);
        }
        
        if (_trail != null)
        {
            _trail.enabled = false;
        }
        
        if (!isGrounded)
        {
            currentAirDashes++;
        }
        
        // فقط اگر روی زمین هستیم کول داون اعمال می‌شود
        if (isGrounded)
        {
            yield return new WaitForSeconds(dashCooldown);
        }
        
        canDash = true;
    }

    private void ResetAirDashes()
    {
        if (isGrounded && resetAirDashOnGround)
        {
            currentAirDashes = 0;
        }
    }

    public void push(float Power)
    {
        _rb.velocity = new Vector2(transform.position.x, Power);
    }

    private void ToggleGravity()
    {
        Flip();
        if (isGravityInverted)
        {
            _rb.gravityScale = invertedGravityScale;
            transform.localRotation = Quaternion.Euler(180, 0, 0);
        }
        else
        {
            _rb.gravityScale = defaultGravityScale;
            transform.localRotation = Quaternion.identity;
        }
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
        Vector2 groundRayDirection = isGravityInverted ? Vector2.up : Vector2.down;
        Vector2 rayOrigin = new Vector2(transform.position.x - 0.55f * faceDirection, transform.position.y);

        bool groundedOnTrap = Physics2D.Raycast(rayOrigin, groundRayDirection, groundCheckDistace, whatIstrap);
        bool groundedOnGround = Physics2D.Raycast(rayOrigin, groundRayDirection, groundCheckDistace, whatIsground);

        isGrounded = groundedOnTrap || groundedOnGround;
        
        if (isGrounded)
        {
            ResetAirDashes();
        }
    }

    private void WallDetected()
    {
        isWallDitected = Physics2D.Raycast(transform.position, new Vector2(1, 0) * faceDirection, wallCheckDistance, whatIsground);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(enemyCheck.position,EnemyCheckRadius);
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + wallCheckDistance * faceDirection, transform.position.y));

        Vector2 groundRayDirection = isGravityInverted ? Vector2.up : Vector2.down;
        Vector2 rayOrigin = new Vector2(transform.position.x - (float)0.55 * faceDirection, transform.position.y);
        Gizmos.DrawLine(rayOrigin, rayOrigin + groundRayDirection * groundCheckDistace);
    }

    private void CanDoubleJump()
    {
        if (isGrounded || isWallDitected)
            canDoublejump = true;
    }

    private void JumpingAnimation()
    {
        _anim.SetBool("isJumping", _rb.velocity.y != 0 && !isGrounded);
    }

    private void RunningAnimation()
    {
        IsPlayerRunning();
        _anim.SetBool("isRunning", isRunning);
    }

    private void IsPlayerRunning()
    {
        isRunning = _rb.velocity.x != 0 && isGrounded && !isWallDitected;
    }

    private void Jump()
    {
        float actualJumpForce = isGravityInverted ? -jumpForce : jumpForce;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isWallDitected)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, actualJumpForce);
            _anim.SetBool("isJumping", true);
        }
        else if (Input.GetKeyDown(KeyCode.Space) && !isGrounded && canDoublejump)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, actualJumpForce);
            _anim.SetBool("isJumping", true);
            canDoublejump = false;
        }
    }

    public void MoveX()
    {
        if ((isWallDitected && !isGrounded) || isWallJumping)
            return;

        float moveInput = Input.GetAxis("Horizontal");
        _rb.velocity = new Vector2(moveInput * speed, _rb.velocity.y);
    }

    // public void KnockBack(float sourceDamageXposition)
    // {
    //     float KnockBackdir = transform.position.x < sourceDamageXposition ? -1 : 1;
    //
    //    // if (isKnocked) return;
    //
    //     StartCoroutine(KnockBackRoutin());
    //     _anim.SetTrigger("Hit");
    //     _rb.velocity = new Vector2(knockbackPower.x * KnockBackdir, knockbackPower.y);
    // }

    // IEnumerator KnockBackRoutin()
    // {
    //     canBeKnocked = false;
    //     isKnocked = true;
    //     yield return new WaitForSeconds(knockbackDuration);
    //     canBeKnocked = true;
    //     isKnocked = false;
    // }

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
        if (allowWallSlide && isWallDitected && Input.GetKey(KeyCode.S))
        {
            _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y * 3f);
        }
    }

    private void WallJump()
    {
        if (!allowWallSlide) return;

        if (isWallDitected && Input.GetKeyDown(KeyCode.Space) && !isGrounded)
        {
            Flip();
            StartCoroutine(WallJumpingTime());
            _rb.velocity = new Vector2(wallJumpForce.x * 0.1f * -faceDirection, wallJumpForce.y * 0.1f);
            jumpcount--;
        }

        if (isWallDitected && Input.GetKeyDown(KeyCode.Space) && (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A)) && !isGrounded)
        {
            Flip();
            StartCoroutine(WallJumpingTime());
            _rb.velocity = new Vector2(wallJumpForce.x * -faceDirection, wallJumpForce.y);
            jumpcount--;
        }

        if (isWallDitected && Input.GetKeyDown(KeyCode.Space) && Input.GetKey(KeyCode.W) && !isGrounded)
        {
            Flip();
            StartCoroutine(WallJumpingTime());
            _rb.velocity = new Vector2(wallJumpForce.x * 0.3f * -faceDirection, wallJumpForce.y * 1.2f);
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
        faceDirection = isFacingRight ? 1 : -1;
    }

    public void Flip()
    {
        transform.Rotate(0, 180, 0);
        isFacingRight = !isFacingRight;
    }

    void ChangeFacing()
    {
        if (!isChangeFacingActive) return;

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