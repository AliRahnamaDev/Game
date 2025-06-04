using UnityEngine;

public class AttackSource : MonoBehaviour
{
    [Header("Attack Settings")]
    public float damageAmount = 10f;
    public string targetTag = "Enemy"; // یا "Player" اگر بخوای یه انمی هم بزنه
    public GameObject attacker; // معمولاً owner

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(targetTag))
            return;

        Damageable damageable = other.GetComponent<Damageable>();
        if (damageable != null && other.gameObject != attacker)
        {
            damageable.TakeDamage(damageAmount, attacker);
        }
    }
}