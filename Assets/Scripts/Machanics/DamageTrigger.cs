using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damageAmount = 10f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // اول تلاش کن مستقیم Damageable بگیری
        Damageable damageable = other.GetComponent<Damageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damageAmount, gameObject);
            return;
        }

        // اگر Damageable نبود، شاید PlayerForm داشته باشه؟
        PlayerForm form = other.GetComponent<PlayerForm>();
        if (form != null && form.sharedHealth != null)
        {
            form.ReceiveDamage(damageAmount);
        }
    }
}


