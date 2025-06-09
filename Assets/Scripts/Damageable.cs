using UnityEngine;

public class Damageable : MonoBehaviour
{
    [Header("Health Settings")]
    public float health = 100f;
    public float dieAnimationDuration = 0.7f; // زمان نمایش انیمیشن مرگ

    private Animator animator;
    public bool isDead = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(float amount, GameObject source = null)
    {
        if (isDead) return;

        health -= amount;

        string sourceName = source ? source.name : "Unknown";
        Debug.Log($"{gameObject.name} took {amount} damage from {sourceName}");

        if (health > 0)
        {
            SetAnimatorBoolSafe("isHurted", true);
            StartCoroutine(ResetBool("isHurted", 0.2f)); // بعد از 0.2 ثانیه خاموش شود
        }
        else
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        SetAnimatorBoolSafe("isDied", true);

        SendMessage("OnDeath", SendMessageOptions.DontRequireReceiver);

        var respawnable = GetComponent<IRespawnable>();
        if (respawnable != null)
        {
            respawnable.OnDeath(); // پلیرها
        }
        else
        {
            // انمی‌ها، حذف خودکار بعد از پایان انیمیشن مرگ
            Destroy(gameObject, dieAnimationDuration);
        }
        AudioManager.Instance.PlaySFX(SFXType.Die);
    }



    private void SetAnimatorBoolSafe(string param, bool value)
    {
        if (animator && HasBool(animator, param))
        {
            animator.SetBool(param, value);
        }
    }

    private bool HasBool(Animator anim, string name)
    {
        foreach (var p in anim.parameters)
        {
            if (p.name == name && p.type == AnimatorControllerParameterType.Bool)
                return true;
        }
        return false;
    }

    private System.Collections.IEnumerator ResetBool(string param, float delay)
    {
        yield return new WaitForSeconds(delay);
        SetAnimatorBoolSafe(param, false);
    }
    
    public void Revive(float newHealth = 100f)
    {
        health = newHealth;
        isDead = false;
        SetAnimatorBoolSafe("isDied", false); // انیمیشن مرگ خاموش بشه
    }

}
