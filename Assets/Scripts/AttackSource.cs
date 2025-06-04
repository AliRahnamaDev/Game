using UnityEngine;

public class AttackSource : MonoBehaviour
{
    public float damageAmount = 10f;
    public string sourceTag = "Player"; // اگر از پلیر اومده

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy"))
            return;

        Debug.Log($"Enemy hit by {sourceTag}! Damage: {damageAmount}");

        Damageable damageable = other.GetComponent<Damageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damageAmount);
        }
    }
}