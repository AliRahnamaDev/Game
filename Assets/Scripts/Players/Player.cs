using System.Collections;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour,IKnockbackable,IRespawnable
{
    public bool canMoveHorizontally = true;
    [SerializeField] private LayerMask whatIsEnemy;
    [SerializeField] private float EnemyCheckRadius;
    [SerializeField] private Transform enemyCheck;
    
    [Header("Control Keys")]
    public KeyCode moveLeftKey = KeyCode.A;
    public KeyCode moveRightKey = KeyCode.D;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode dashKey = KeyCode.LeftShift;
    public KeyCode gravityInvertKey = KeyCode.M;
    public KeyCode wallSlideFastKey = KeyCode.S;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    #region Def

    [Header("          *********Abilities*********")]
    public bool allowWallSlide = true;
    public bool allowGravityInvert = true;
    public bool allowDash = true;

    [Header("          *********Dash Settings*********")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.5f;
    public bool canDash = true;
    public bool isDashing = false;
    public int airDashCount = 2;
    public int currentAirDashes = 0;
    public bool resetAirDashOnGround = true;
    public Color dashTrailColor = Color.white;
    public bool omnidirectionalDash = true;
    public float dashEndVerticalMultiplier = 0.5f;
    public bool canAddDash = false;

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
        damageable = GetComponent<Damageable>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _bc = GetComponent<BoxCollider2D>();
        _trail = GetComponent<TrailRenderer>();
        defaultGravityScale = 4.5f;
        
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
        if(!isDashing) MoveX();

        if (allowGravityInvert && Input.GetKeyDown(gravityInvertKey))
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
        //WallSlidingFaster();
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
                if (_rb.velocity.y < 0 && transform.position.y > newEnemy.transform.position.y + 0.2f)
                {
                    _rb.velocity = new Vector2(_rb.velocity.x, 0);
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
            float horizontalInput = 0f;
            if (Input.GetKey(moveLeftKey)) horizontalInput -= 1f;
            if (Input.GetKey(moveRightKey)) horizontalInput += 1f;
            
            float verticalInput = Input.GetAxisRaw("Vertical");
            
            dashDirection = new Vector2(horizontalInput, verticalInput).normalized;
            
            if (dashDirection == Vector2.zero)
            {
                dashDirection = isFacingRight ? Vector2.right : Vector2.left;
            }
        }
        else
        {
            float dashDirectionX = 0f;
            if (Input.GetKey(moveLeftKey)) dashDirectionX -= 1f;
            if (Input.GetKey(moveRightKey)) dashDirectionX += 1f;
            
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

    // public void Die()
    // {
    //     Destroy(gameObject);
    //     GameObject newFx = Instantiate(DeadVfx, transform.position, Quaternion.identity);
    //     Destroy(newFx, 0.3f);
    // }

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

        if (Input.GetKeyDown(jumpKey) && isGrounded && !isWallDitected)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, actualJumpForce);
            _anim.SetBool("isJumping", true);
        }
        else if (Input.GetKeyDown(jumpKey) && !isGrounded && canDoublejump)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, actualJumpForce);
            _anim.SetBool("isJumping", true);
            canDoublejump = false;
        }
    }

    public void MoveX()
    {
        if ((isWallDitected && !isGrounded) || isWallJumping || !canMoveHorizontally)
            return;

        float moveInput = 0f;
        if (Input.GetKey(moveLeftKey)) moveInput -= 1f;
        if (Input.GetKey(moveRightKey)) moveInput += 1f;
        
        _rb.velocity = new Vector2(moveInput * speed, _rb.velocity.y);
    }

    private void DoubleJumpAfterWallJumping()
    {
        if (isWallJumping && !isWallDitected && Input.GetKeyDown(jumpKey))
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

    // private void WallSlidingFaster()
    // {
    //     if (allowWallSlide && isWallDitected && Input.GetKey(wallSlideFastKey))
    //     {
    //         _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y * 3f);
    //     }
    // }

    private void WallJump()
    {
        if (!allowWallSlide) return;

        if (isWallDitected && Input.GetKeyDown(jumpKey) && !isGrounded)
        {
            Flip();
            StartCoroutine(WallJumpingTime());
            _rb.velocity = new Vector2(wallJumpForce.x * 0.1f * -faceDirection, wallJumpForce.y * 0.1f);
            jumpcount--;
        }

        if (isWallDitected && Input.GetKeyDown(jumpKey) && (Input.GetKey(moveRightKey) || Input.GetKey(moveLeftKey)) && !isGrounded)
        {
            Flip();
            StartCoroutine(WallJumpingTime());
            _rb.velocity = new Vector2(wallJumpForce.x * -faceDirection, wallJumpForce.y);
            jumpcount--;
        }

        if (isWallDitected && Input.GetKeyDown(jumpKey) && Input.GetKey(KeyCode.W) && !isGrounded)
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

            float slideDirection = isGravityInverted ? 1f : -1f;
            _rb.velocity = new Vector2(_rb.velocity.x, slideDirection * slidingSpeed);

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

        if (Input.GetKey(moveLeftKey) && isFacingRight)
        {
            Flip();
        }
        else if (Input.GetKey(moveRightKey) && !isFacingRight)
        {
            Flip();
        }
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
    
    
    private Damageable damageable;
    
        public bool IsDead { get; private set; } = false;
        public GameObject respawnPrefab;

        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void OnDeath()
        {
            if (IsDead) return;

            IsDead = true;

            //animator.SetBool("isDied", true);
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            StartCoroutine(RespawnCoroutine());
        }
        
        
        public GameObject GetGameObject() => gameObject;

        private IEnumerator RespawnCoroutine()
        {
            spriteRenderer.enabled = false;
            _rb.bodyType = RigidbodyType2D.Static;
            boxCollider.enabled = false;

            yield return new WaitForSeconds(15);

            Transform otherPlayer = FindOtherAlivePlayer();
            if (otherPlayer != null)
            {
                transform.position = otherPlayer.position;
                gameObject.SetActive(true);

                spriteRenderer.enabled = true;
                _rb.bodyType = RigidbodyType2D.Dynamic;
                boxCollider.enabled = true;

                damageable.Revive(); // 👈 فقط این کافیه
                IsDead = false;
            }
            else
            {
                UIManager.Instance.ShowGameOverMenu();
            }
        }



        private Transform FindOtherAlivePlayer()
        {
            IRespawnable[] players = FindObjectsOfType<MonoBehaviour>().OfType<IRespawnable>().ToArray();
            foreach (var player in players)
            {
                if (!player.IsDead && player.GetGameObject() != this.gameObject)
                    return player.GetGameObject().transform;
            }
            return null;
        }
    }
    