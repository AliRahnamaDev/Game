using UnityEngine;
using System.Collections.Generic;

public class Arrow : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool stuck = false;

    [Header("Settings")]
    
    public float embedDepth = 0.1f;
    
    public float destroyDelay = 3f;
    
    public float fadeDuration = 1f;

    [Header("Layer Behavior Lists")]
    public List<string> instantDestroyLayers;
    public List<string> fadeDestroyLayers;
    public List<string> ignoreCollisionLayers;

    private SpriteRenderer spriteRenderer;
    private float fadeTimer = 0f;
    private bool fading = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // اگر تا 3 ثانیه هیچ برخوردی اتفاق نیفتاد نابود شو
        Invoke(nameof(SelfDestructIfUnstuck), 3f);
    }
    void SelfDestructIfUnstuck()
    {
        if (!stuck)
        {
            Debug.Log("Arrow auto-destroyed after timeout");
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (stuck) return;

        string hitLayerName = LayerMask.LayerToName(collision.collider.gameObject.layer);
        Debug.Log($"Arrow hit: {collision.gameObject.name} on layer {hitLayerName}");
        
        if (ignoreCollisionLayers.Contains(hitLayerName))
        {
            Debug.Log("Collision ignored (safe layer)");
            return;
        }
        
        if (instantDestroyLayers.Contains(hitLayerName))
        {
            Debug.Log("Instant destroy layer hit!");
            Destroy(gameObject);
            return;
        }
        
        if (fadeDestroyLayers.Contains(hitLayerName))
        {
            Debug.Log("Fade destroy layer hit!");
            StickArrow(collision);
            Invoke(nameof(StartFadeOut), destroyDelay);
            return;
        }
        //by defualt
        Debug.Log("Uncategorized layer → destroy by default");
        Destroy(gameObject);
    }

    void StickArrow(Collision2D collision)
    {
        stuck = true;

        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        rb.freezeRotation = true;

        Vector3 contactPoint = collision.contacts[0].point;
        Vector3 direction = transform.right * (transform.localScale.x > 0 ? 1 : -1);
        transform.position = contactPoint + (Vector3)(-direction * embedDepth);

        transform.SetParent(collision.transform);
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
