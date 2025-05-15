using UnityEngine;

public class ButtonActivatedPlatform : MonoBehaviour

{

    public Transform[] waypointObjects;
    public float moveSpeed = 2f;
    public float returnSpeed = 1f;
    public PlatformStartButton button;

    private Vector3[] waypointPositions;
    private int currentIndex = 0;
    private bool returning = false;

    public Pause pauseButton;  // متغیر جدید برای توقف موقت

    void Start()
    {
        // ذخیره موقعیت‌های جهانی برای جلوگیری از حرکت همراه پلتفرم
        waypointPositions = new Vector3[waypointObjects.Length];
        for (int i = 0; i < waypointObjects.Length; i++)
        {
            waypointPositions[i] = waypointObjects[i].position;
        }
    }

    void Update()
    {
        if (pauseButton != null && pauseButton.shouldPausePlatform)
            return; // توقف کامل حرکت وقتی پلیر روی دکمه است

        if (button == null || waypointPositions.Length == 0) return;

        if (button.isPressed)
        {
            returning = false; // اگر دوباره روی دکمه رفت، مسیر برگشتی قطع شود

            // حرکت روبه‌جلو
            Vector3 target = waypointPositions[currentIndex];
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, target) < 0.01f)
            {
                if (currentIndex < waypointPositions.Length - 1)
                {
                    currentIndex++;
                }
            }
        }
        else
        {
            returning = true;

            // حرکت به عقب
            Vector3 target = waypointPositions[currentIndex];
            transform.position = Vector3.MoveTowards(transform.position, target, returnSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, target) < 0.01f)
            {
                if (currentIndex > 0)
                {
                    currentIndex--;
                }
            }
        }
    }
}
