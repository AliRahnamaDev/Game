using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Collision Settings")]
    [SerializeField] private LayerMask collisionLayers; // لایه‌هایی که گلوله با آنها برخورد می‌کند
    [SerializeField] private float damageAmount = 1f; // میزان آسیب به دشمن/بازیکن
    [SerializeField] private float destroyDelay = 0.1f; // تاخیر قبل از نابودی پس از برخورد
    [SerializeField] private GameObject hitEffect; // افکت برخورد (اختیاری)

    [Header("Auto Destroy")]
    [SerializeField] private float lifetime = 3f; // زمان خودنابودی اگر به چیزی برخورد نکرد

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // نابودی خودکار پس از مدت زمان مشخص
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // بررسی آیا شی برخوردی در لایه‌های مورد نظر است
        if (((1 << collision.gameObject.layer) & collisionLayers) != 0)
        {
            // اگر شی برخوردی قابل آسیب دیدن است (مثل بازیکن یا دشمن)

            // ایجاد افکت برخورد اگر تنظیم شده باشد
            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }

            // غیرفعال کردن فیزیک و رندرر قبل از نابودی
            if (rb != null) rb.simulated = false;
            if (TryGetComponent(out SpriteRenderer renderer)) renderer.enabled = false;
            if (TryGetComponent(out Collider2D col)) col.enabled = false;

            // نابودی گلوله پس از تاخیر
            Destroy(gameObject, destroyDelay);
        }
    }
}