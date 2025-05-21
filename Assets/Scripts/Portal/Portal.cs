
using System.Collections;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public bool isFacingRight = true;
    private Player player;

    [Header("Portal Pair")]
    public Portal destinationPortal;

    [Header("Settings")]
    public float disableDuration = 0.5f;

    [Header("Player Force Settings")]
    public bool applyForceToPlayer = true;
    public float playerForceAmount = 500f;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    public float minSpeedForForce = 2f; // حداقل سرعت برای اعمال نیرو
    private void OnTriggerEnter2D(Collider2D other)
    {
        Teleportable teleportable = other.GetComponent<Teleportable>();
        Rigidbody2D rb = other.attachedRigidbody;

        if (teleportable == null || rb == null || teleportable.justTeleported || destinationPortal == null)
            return;

        float speed = rb.velocity.magnitude;

        // انتقال جسم به موقعیت پورتال مقصد
        other.transform.position = destinationPortal.transform.position;

        Vector2 exitDirection = destinationPortal.transform.right.normalized;

        // تنظیم سرعت در جهت خروجی با همان اندازه سرعت اولیه
        rb.velocity = exitDirection * speed;

        // اگر سرعت ورودی کمتر از حد است، نیرویی شبیه پلیر اعمال کن
        if (speed < minSpeedForForce)
        {
            rb.AddForce(exitDirection * playerForceAmount, ForceMode2D.Impulse);
        }
        else if (applyForceToPlayer && other.CompareTag("Player"))
        {
            // اگر پلیر است و سرعت بالاتر بود هم نیروی ویژه اعمال کن (مثل قبل)
            rb.AddForce(exitDirection * playerForceAmount, ForceMode2D.Impulse);
            StartCoroutine(CanControlPlayer());
        }

        teleportable.justTeleported = true;
        destinationPortal.StartCoroutine(destinationPortal.ResetTeleportFlagAfterDelay(teleportable));
    }



    private IEnumerator CanControlPlayer()
    {
        player.canBeControlled = false;
        yield return new WaitForSeconds(0.3f);
        player.canBeControlled = true;
    }

    private IEnumerator ResetTeleportFlagAfterDelay(Teleportable teleportable)
    {
        yield return new WaitForSeconds(disableDuration);
        teleportable.justTeleported = false;
    }

    private void PlayerFacing()
    {
        if (isFacingRight && !player.isFacingRight)
        {
            player.Flip();
        }
        else if (!isFacingRight && player.isFacingRight)
        {
            player.Flip();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerFacing();
        }
    }
}
