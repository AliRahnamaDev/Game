using System.Collections.Generic;
using UnityEngine;

public class ShadowManager : MonoBehaviour
{
    public GameObject shadowPrefab;
    public ShadowRecorder recorder;
    public KeyCode startRecordingKey = KeyCode.R;
    public KeyCode spawnShadowKey = KeyCode.T;

    private bool hasRecordedOnce = false; // 🔸 پرچم: آیا ضبط شده؟
    [Header("تنظیمات پخش سایه")]
    public float shadowDuration = 5f; // ← مدت‌زمان دلخواه برای بازپخش سایه

    void Update()
    {
        // شروع ضبط
        if (Input.GetKeyDown(startRecordingKey))
        {
            recorder.StartRecording();
            hasRecordedOnce = true; // ✅ اولین ضبط انجام شده
        }

        // پخش فقط اگر ضبط انجام شده باشد
        if (Input.GetKeyDown(spawnShadowKey))
        {
            if (!hasRecordedOnce)
            {
                Debug.Log("⛔ ابتدا باید ضبط انجام شود (کلید R)");
                return;
            }

            recorder.StopRecording();
            SpawnShadow();
        }
    }

    void SpawnShadow()
    {
        if (shadowPrefab == null)
        {
            Debug.LogWarning("⚠️ Shadow Prefab is not assigned or was destroyed.");
            return;
        }

        List<ShadowState> fullData = recorder.recordedStates;

        if (fullData == null || fullData.Count == 0)
            return;

        float totalDuration = fullData[fullData.Count - 1].time;

        // تعیین زمان شروع بازپخش
        float startTime = Mathf.Max(0f, totalDuration - shadowDuration);

        // فقط داده‌های مربوط به shadowDuration آخر را نگه داریم
        List<ShadowState> trimmedData = fullData.FindAll(state => state.time >= startTime);

        GameObject ghost = Instantiate(shadowPrefab, recorder.transform.position, Quaternion.identity);
        var ghostScript = ghost.GetComponent<ShadowGhost>();
        if (ghostScript != null)
        {
            ghostScript.SetReplay(trimmedData);
        }
    }


}