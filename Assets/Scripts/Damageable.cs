using UnityEngine;

public class Damageable : MonoBehaviour
{
    [Header("Health Settings")]
    public float health = 100f;
    public float dieAnimationDuration = 0.7f; // زمان نمایش انیمیشن مرگ

    private Animator animator;
    private bool isDead = false;

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

        // اطلاع‌رسانی به دیگر اسکریپت‌ها بدون وابستگی
        SendMessage("OnDeath", SendMessageOptions.DontRequireReceiver);
        

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb) rb.velocity = Vector2.zero;

        // حذف نهایی پس از اجرای انیمیشن
        Destroy(gameObject, dieAnimationDuration);
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
}
