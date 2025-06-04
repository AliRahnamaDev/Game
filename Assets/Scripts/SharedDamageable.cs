// روی یک گیم‌اوبجکت مرکزی که Destroy نمی‌شه قرار بگیره

using UnityEngine;

public class SharedDamageable : MonoBehaviour
{
    public float health = 100f;

    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log($"Shared health is now {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player died in any form");
        Destroy(gameObject); // یا مدیریت بهتر مثل بازگشت به منوی اصلی
    }
}