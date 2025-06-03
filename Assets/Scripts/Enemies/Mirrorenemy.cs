using UnityEngine;
using System.Collections;

public class Mirrorenemy : MonoBehaviour
{
    [Header("Player Reference")]
    public Transform player;
    public Player playerScript;

    [Header("Settings")]
    public float activateDistance = 10f;
    public float moveSpeed = 5f;
    public float jumpForce = 6f;
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public int maxAirDashes = 2;

    private Rigidbody2D rb;
    private bool isGrounded = true;
    private int currentAirDashes = 0;
    private bool isDashing = false;
    private bool canDash = true;

    private Animator anim;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        float distance = Vector2.Distance(player.position, transform.position);
        if (distance > activateDistance) return;

        if (!isDashing)
        {
            MirrorMovement();
            MirrorJump();
        }
        MirrorDash();
        AnimateRun();
    }

    void MirrorMovement()
    {
        float playerInput = Input.GetAxisRaw("Horizontal");
        float mirroredInput = -playerInput;
        rb.velocity = new Vector2(mirroredInput * moveSpeed, rb.velocity.y);

        // Flip to face direction
        if (mirroredInput > 0)
            transform.localScale = new Vector3(-1, 1, 1); // چپ رفتن
        else if (mirroredInput < 0)
            transform.localScale = new Vector3(1, 1, 1); // راست رفتن
    }

    void MirrorJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                isGrounded = false;
            }
            else if (playerScript.canDoublejump && currentAirDashes < maxAirDashes)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                currentAirDashes++;
            }
        }
    }

    void MirrorDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && (isGrounded || currentAirDashes < maxAirDashes))
        {
            StartCoroutine(DashRoutine());
        }
    }

    IEnumerator DashRoutine()
    {
        isDashing = true;
        canDash = false;

        float playerInput = Input.GetAxisRaw("Horizontal");
        float mirrorInput = -playerInput;

        Vector2 dashDir;

        if (playerScript.omnidirectionalDash)
        {
            // ورودی دو بعدی پلیر
            float px = Input.GetAxisRaw("Horizontal");
            float py = Input.GetAxisRaw("Vertical");

            // معکوسش کن
            dashDir = new Vector2(-px, py).normalized;

            // اگر پلیر ساکن بود، بر اساس جهت فعلی دشمن
            if (dashDir == Vector2.zero)
            {
                dashDir = transform.localScale.x > 0 ? Vector2.left : Vector2.right;
            }
        }
        else
        {
            // Dash فقط در جهت افقی معکوس پلیر
            dashDir = mirrorInput == 0 ?
                (transform.localScale.x > 0 ? Vector2.left : Vector2.right) :
                new Vector2(mirrorInput, 0f).normalized;
        }

        rb.velocity = dashDir * dashSpeed;

        yield return new WaitForSeconds(dashDuration);
        isDashing = false;

        if (!isGrounded)
            currentAirDashes++;

        yield return new WaitForSeconds(0.3f);
        canDash = true;
    }

    void AnimateRun()
    {
        if (rb.velocity.y != 0)
        {
            anim.SetBool("isRunning", true);
        }
        else
        {
            anim.SetBool("isRunning", false);
        }
    }


    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Ground"))
        {
            isGrounded = true;
            currentAirDashes = 0;
        }
    }
}
