using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        
        SharedDamageable shared = other.GetComponentInParent<SharedDamageable>();
        if (shared != null)
        {
            shared.TakeDamage(shared.health + 999); // Kill instantly
            return;
        }
        
        Damageable dmg = other.GetComponent<Damageable>();
        if (dmg != null)
        {
            dmg.TakeDamage(dmg.health + 999); // Kill instantly
        }
    }
}