using UnityEngine;
using UnityEngine.UI;
using TMPro; // اگر از TextMeshPro استفاده می‌کنی

public class SliderHealthBarUI : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text livesText; // 🔺 نمایش جان‌ها
    private SharedDamageable target;
    private float maxHealth;

    private bool isInitialized = false;

    private void Start()
    {
        if (slider != null)
            slider.interactable = false;
    }

    private void Update()
    {
        if (target == null) return;

        // مقداردهی اولیه
        if (!isInitialized)
        {
            maxHealth = target.health;
            slider.maxValue = maxHealth;
            slider.value = maxHealth;
            isInitialized = true;

            Debug.Log($"✅ HealthBar for {gameObject.name} initialized with maxHealth: {maxHealth}");
        }

        // آپدیت مقدار سلامت
        slider.value = Mathf.Clamp(target.health, 0, maxHealth);

        // 🔺 آپدیت lives در UI
        if (livesText != null)
        {
            livesText.text = "♥ " + target.totalLives.ToString();
        }
    }

    public void SetTarget(SharedDamageable shared)
    {
        target = shared;
        isInitialized = false;
    }
}