using UnityEngine;

public class HealthPickUp : MonoBehaviour
{
    [SerializeField] private float healAmount = 30f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // فقط اگر به فرمی برخورد کردیم (مثلاً PlayerForm1 یا PlayerForm2)
        if (collision.transform.parent != null)
        {
            SharedDamageable damageable = collision.transform.parent.GetComponent<SharedDamageable>();

            if (damageable != null)
            {
                damageable.Heal(healAmount);
                Destroy(gameObject); // آیتم حذف شود
            }
        }
    }
}