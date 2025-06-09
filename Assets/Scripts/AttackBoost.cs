using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AttackBoost : MonoBehaviour
{
    [Tooltip("چند برابر شود؟ مثلاً 2 = دو برابر")]
    public float multiplier = 2f;

    [Tooltip("مدت زمان تقویت")]
    public float duration = 5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Transform attackChild = other.transform.Find("AttackSource");
        if (attackChild == null)
        {
            Debug.LogWarning($"❌ No child named 'AttackSource' in {other.name}");
            return;
        }

        AttackSource attackSource = attackChild.GetComponent<AttackSource>();
        if (attackSource == null)
        {
            Debug.LogWarning($"❌ No AttackSource script on child 'AttackSource' of {other.name}");
            return;
        }

        // اجرای Coroutine روی همین آیتم
        StartCoroutine(ApplyBoostAndDestroy(attackChild.gameObject, attackSource));
        Destroy(gameObject); // حذف آیتم تقویتی
    }

    private System.Collections.IEnumerator ApplyBoostAndDestroy(GameObject attackObj, AttackSource attackSource)
    {
        float originalDamage = attackSource.damageAmount;
        attackSource.damageAmount *= multiplier;

        Debug.Log($"🔥 Boost applied! New damage: {attackSource.damageAmount}");

        // فعال‌سازی اگر غیرفعاله
        if (!attackObj.activeSelf)
            attackObj.SetActive(true);

        yield return new WaitForSeconds(duration);

        // بازگشت مقدار
        attackSource.damageAmount = originalDamage;
        Debug.Log($"🧊 Boost ended. Restoring and destroying AttackSource");

        // غیر فعال و حذف
        attackObj.SetActive(false);
        Destroy(attackObj);
    }
}