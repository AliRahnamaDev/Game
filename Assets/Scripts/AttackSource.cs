using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AttackSource : MonoBehaviour
{
    [Header("Attack Settings")]
    public float damageAmount = 10f;
    public string targetTag = "Enemy"; // یا "Player" اگر این تیر از طرف دشمن باشه
    [HideInInspector] public GameObject attacker; // معمولاً ارچر یا شمشیرزن

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(targetTag))
            return;

        Damageable damageable = other.GetComponent<Damageable>();
        if (damageable != null)
        {
            // جلوگیری از friendly fire
            if (attacker != null && other.gameObject == attacker)
            {
                Debug.Log("Hit ignored: self-collision");
                return;
            }

            // اعمال دمیج
            Debug.Log($"[AttackSource] Hit {other.name} → Damage: {damageAmount}");
            damageable.TakeDamage(damageAmount, attacker);
        }
    }

    /// <summary>
    /// مقداردهی تیر یا ضربه از بیرون (مثلاً موقع Instantiate)
    /// </summary>
    public void Initialize(GameObject attackerObj)
    {
        attacker = attackerObj;
    }
}
