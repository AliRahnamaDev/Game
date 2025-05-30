using System.Collections.Generic;
using UnityEngine;

public class ShadowManager : MonoBehaviour
{
    [Header("Prefabs & References")]
    public GameObject shadowPrefab;
    public ShadowRecorder recorder;

    [Header("Key Bindings")]
    public KeyCode startRecordingKey = KeyCode.R;
    public KeyCode spawnShadowKey   = KeyCode.T;

    [Header("Playback Settings")]
    public float shadowDuration = 5f; // ثانیه‌های آخر ضبط برای پخش سایه

    private bool canSpawn = false;  // فقط وقتی R زده شد، اجازه T داریم

    void Update()
    {
        // ۱) با R ضبط را شروع کن و اجازه پخش بده
        if (Input.GetKeyDown(startRecordingKey))
        {
            recorder.StartRecording();
            canSpawn = true;    // پس از R، یک بار اجازه T صادر می‌شود
        }

        // ۲) با T فقط اگر اجازه داشته باشیم پخش کن
        if (Input.GetKeyDown(spawnShadowKey))
        {
            if (!canSpawn)
            {
                Debug.Log("⛔ ابتدا باید کلید R را بزنید تا ضبط شروع شود.");
                return;
            }

            recorder.StopRecording();
            SpawnShadow();
            canSpawn = false;   // پس از یک بار T، اجازه دوباره تا R بعدی لغو می‌شود
        }
    }

    void SpawnShadow()
    {
        if (shadowPrefab == null)
        {
            Debug.LogWarning("⚠️ Prefab سایه تنظیم نشده یا نابود شده.");
            return;
        }

        List<ShadowState> fullData = recorder.recordedStates;
        if (fullData == null || fullData.Count == 0)
            return;

        // محاسبه مدت‌کل ضبط و نقطه شروع پخش
        float totalDuration = fullData[fullData.Count - 1].time;
        float startTime = Mathf.Max(0f, totalDuration - shadowDuration);

        // فیلتر کردن داده‌های مربوط به shadowDuration ثانیه آخر
        List<ShadowState> trimmedData = fullData.FindAll(state => state.time >= startTime);

        // Instantiate سایه و دادن داده‌ها
        GameObject ghost = Instantiate(shadowPrefab, recorder.transform.position, Quaternion.identity);
        var ghostScript = ghost.GetComponent<ShadowGhost>();
        if (ghostScript != null)
        {
            ghostScript.SetReplay(trimmedData);
        }
    }
}
