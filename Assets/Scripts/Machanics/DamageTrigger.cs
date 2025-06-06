using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damageAmount = 10f;

    [Header("Knockback Settings")]
    public bool enableKnockback = true;
    public float horizontalForce = 5f;
    public float verticalForce = 3f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // بررسی آیا پلیر در حال دفاع است
        var blockingData = other.GetComponent<IBlockable>();
        bool isBlocking = false;
        float dmgMultiplier = 1f;
        float knockbackMultiplier = 1f;

        if (blockingData != null && blockingData.IsBlocking())
        {
            isBlocking = true;
            dmgMultiplier = blockingData.GetDamageReductionMultiplier();
            knockbackMultiplier = blockingData.GetKnockbackReductionMultiplier();
        }

        float finalDamage = damageAmount * dmgMultiplier;

        // Damageable
        Damageable damageable = other.GetComponent<Damageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(finalDamage, gameObject);
            TryApplyKnockback(other, knockbackMultiplier);
            return;
        }

        // PlayerForm (shared health)
        PlayerForm form = other.GetComponent<PlayerForm>();
        if (form != null && form.sharedHealth != null)
        {
            form.ReceiveDamage(finalDamage);
            TryApplyKnockback(other, knockbackMultiplier);
        }
    }


    void TryApplyKnockback(Collider2D target, float knockbackMultiplier = 1f)
    {
        Rigidbody2D rb = target.attachedRigidbody;
        if (rb == null) return;

        float directionX = (target.transform.position.x > transform.position.x) ? 1f : -1f;
        Vector2 force = new Vector2(directionX * horizontalForce, verticalForce) * knockbackMultiplier;

        rb.velocity = Vector2.zero;
        rb.AddForce(force, ForceMode2D.Impulse);

        IKnockbackable knockbackable = target.GetComponent<IKnockbackable>();
        if (knockbackable != null)
        {
            knockbackable.PauseHorizontalMovement(0.5f);
        }
    }


}