using UnityEngine;
using UnityEngine.UI;

public class SliderHealthBarUI : MonoBehaviour
{
    [SerializeField] private Slider slider;
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

        if (!isInitialized)
        {
            maxHealth = target.health;
            slider.maxValue = maxHealth;
            slider.value = maxHealth;
            isInitialized = true;

            Debug.Log($"✅ HealthBar for {gameObject.name} initialized with maxHealth: {maxHealth}");
        }

        slider.value = Mathf.Clamp(target.health, 0, maxHealth);
    }

    public void SetTarget(SharedDamageable shared)
    {
        target = shared;
        isInitialized = false;
    }
}