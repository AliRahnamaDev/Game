using UnityEngine;

public class Damageable : MonoBehaviour
{
    public float health = 100f;

    public void TakeDamage(float damage)
    {
        Debug.Log($"{gameObject.name} took {damage} damage!");
    }
}

