using UnityEngine;
using System.Collections;

public class HealthBarInitializer : MonoBehaviour
{
    public GameObject healthUIPrefab;
    public Camera playerCamera;

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        var canvas = Instantiate(healthUIPrefab);
        canvas.GetComponent<Canvas>().worldCamera = playerCamera;

        var healthUI = canvas.GetComponentInChildren<SliderHealthBarUI>();
        var shared = GetComponent<SharedDamageable>();
        healthUI.SetTarget(shared);
    }
}