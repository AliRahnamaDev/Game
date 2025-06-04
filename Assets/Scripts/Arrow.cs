using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool stuck = false;

    [Tooltip("مقدار فرو رفتن تیر به داخل زمین")]
    public float embedDepth = 0.1f;

    [Tooltip("مدت زمان تا شروع محو شدن")]
    public float destroyDelay = 3f;

    [Tooltip("مدت زمان محو شدن")]
    public float fadeDuration = 1f;

    private SpriteRenderer spriteRenderer;
    private float fadeTimer = 0f;
    private bool fading = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (stuck) return;

        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            stuck = true;

            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
            rb.freezeRotation = true;

            Vector3 contactPoint = collision.contacts[0].point;
            Vector3 direction = transform.right * (transform.localScale.x > 0 ? 1 : -1);
            transform.position = contactPoint + (Vector3)(-direction * embedDepth);

            transform.SetParent(collision.transform);

            // شروع تایمر برای محو شدن
            Invoke(nameof(StartFadeOut), destroyDelay);
        }
    }

    void StartFadeOut()
    {
        fading = true;
    }

    void Update()
    {
        if (fading)
        {
            fadeTimer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, fadeTimer / fadeDuration);

            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = alpha;
                spriteRenderer.color = color;
            }

            if (fadeTimer >= fadeDuration)
            {
                Destroy(gameObject);
            }
        }
    }
}