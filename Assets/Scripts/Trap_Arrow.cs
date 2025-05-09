using System.Collections;
using UnityEngine;

public class Trap_Arrow : MonoBehaviour
{
    public float rotationspeed = 100f;
    public float launchForce = 10f;
    public float PlayercantControlltime = 0.5f;
    public float ArrowDisabletime = 1f;
    public float reactivationCheckRadius = 0.5f;

    private SpriteRenderer spriteRenderer;
    private Collider2D arrowCollider;
    private Transform playerTransform;
    private Player player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Player>();
        playerTransform = player?.transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        arrowCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        transform.Rotate(0, 0, rotationspeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(HandleArrow(other));
        }
    }

    private IEnumerator HandleArrow(Collider2D other)
    {
        // 1. غیر فعال کردن فلش
        spriteRenderer.enabled = false;
        arrowCollider.enabled = false;

        // 2. اعمال نیرو
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 direction = transform.up.normalized;
            rb.velocity = Vector2.zero;
            rb.AddForce(direction * launchForce, ForceMode2D.Impulse);
        }

        // 3. غیرفعال کردن کنترل پلیر
        if (player != null)
        {
            player.canBeControlled = false;
            yield return new WaitForSeconds(PlayercantControlltime);
            player.canBeControlled = true;
        }

        // 4. صبر تا زمان اولیه سپری شود
        yield return new WaitForSeconds(ArrowDisabletime);

        // 5. اگر پلیر هنوز در ناحیه فلش است، صبر کن تا خارج شود
        while (IsPlayerInside())
        {
            yield return null;
        }
        // 6. فعال‌سازی مجدد
        spriteRenderer.enabled = true;
        arrowCollider.enabled = true;
    }

    private bool IsPlayerInside()
    {
        if (playerTransform == null)
            return false;

        Collider2D hit = Physics2D.OverlapCircle(transform.position, reactivationCheckRadius, LayerMask.GetMask("Player"));
        return hit != null;
    }
}