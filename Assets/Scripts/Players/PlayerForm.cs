using UnityEngine;

public class PlayerForm : MonoBehaviour
{
    public SharedDamageable sharedHealth;

    public void ReceiveDamage(float amount)
    {
        Debug.Log($"{gameObject.name} received {amount} damage");
        sharedHealth.TakeDamage(amount);
    }

}

