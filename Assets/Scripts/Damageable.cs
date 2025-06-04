using UnityEngine;

public class Damageable : MonoBehaviour
{
    [Header("Health Settings")]
    public float health = 100f;

    public void TakeDamage(float amount, GameObject source = null)
    {
        health -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage from {source?.name}");

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} died!");
        Destroy(gameObject);
    }
}