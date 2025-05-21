using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ReverseTime : MonoBehaviour
{
    [Header("Dettings")]
    [SerializeField] private float maxRewindTime = 5f; // حداکثر زمان برگشت
    [SerializeField] private float recordInterval = 0.02f; // فاصله زمانی بین ذخیره‌ها
    [SerializeField] private float rewindSpeed = 1f; // سرعت برگشت زمان
    [SerializeField] private float positionThreshold = 0.01f; // حداقل تغییر موقعیت برای ذخیره
    [SerializeField] private float velocityThreshold = 0.1f; // حداقل سرعت برای تشخیص حرکت

    [Header("Infos")]
    [SerializeField, ReadOnly] private float currentRewindTime = 0f; // زمان واقعی برگشت
    [SerializeField, ReadOnly] private int recordedPoints = 0; // تعداد نقاط ذخیره شده
    [SerializeField, ReadOnly] private bool isObjectMoving = false; // وضعیت حرکت جسم

    [Header("Effects")]
    [SerializeField] private GameObject rewindEffect; // افکت بصری زمان‌برگردان
    [SerializeField] private Color rewindColor = new Color(1f, 0.5f, 0.5f, 0.5f); // رنگ زمان‌برگردان

    private List<TimePoint> timePoints;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isRewinding = false;
    private float rewindTimer = 0f;
    private Vector2 lastPosition;
    private bool wasMoving = false;
    private bool isRewindKeyHeld = false;
    private TimePoint lastRewindPoint;
    private bool reachedEndOfRewind = false;

    private void Start()
    {
        timePoints = new List<TimePoint>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
        
        lastPosition = transform.position;
        InvokeRepeating(nameof(RecordTimePoint), 0f, recordInterval);
    }

    private void Update()
    {
        // کنترل زمان‌برگردان با کلید R
        if (Input.GetKeyDown(KeyCode.R))
        {
            isRewindKeyHeld = true;
            reachedEndOfRewind = false;
            StartRewind();
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            isRewindKeyHeld = false;
            if (reachedEndOfRewind && lastRewindPoint != null)
            {
                // اعمال سرعت و چرخش آخرین نقطه به جسم
                rb.velocity = lastRewindPoint.velocity;
                rb.angularVelocity = lastRewindPoint.angularVelocity;
            }
            StopRewind();
        }

        // نمایش افکت زمان‌برگردان
        if (isRewinding && rewindEffect != null)
        {
            rewindEffect.SetActive(true);
        }
        else if (rewindEffect != null)
        {
            rewindEffect.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        if (isRewinding && isRewindKeyHeld)
        {
            RewindTime();
        }
        else if (!isRewindKeyHeld && reachedEndOfRewind)
        {
            StopRewind();
        }
    }

    private void RecordTimePoint()
    {
        if (isRewinding) return;

        // بررسی حرکت جسم با استفاده از سرعت و تغییر موقعیت
        isObjectMoving = rb.velocity.magnitude > velocityThreshold || 
                        Vector2.Distance(transform.position, lastPosition) > positionThreshold;

        if (isObjectMoving)
        {
            TimePoint newPoint = new TimePoint(
                transform.position,
                transform.rotation,
                rb.velocity,
                rb.angularVelocity
            );
            
            timePoints.Insert(0, newPoint);
            lastPosition = transform.position;
            wasMoving = true;
            currentRewindTime += recordInterval;
            recordedPoints = timePoints.Count;
        }
        else if (wasMoving)
        {
            // ذخیره نقطه توقف
            TimePoint stopPoint = new TimePoint(
                transform.position,
                transform.rotation,
                Vector2.zero,
                0f
            );
            timePoints.Insert(0, stopPoint);
            wasMoving = false;
            recordedPoints = timePoints.Count;
        }

        // حذف نقاط قدیمی بر اساس زمان واقعی حرکت
        while (timePoints.Count > 0 && currentRewindTime > maxRewindTime)
        {
            timePoints.RemoveAt(timePoints.Count - 1);
            currentRewindTime -= recordInterval;
            recordedPoints = timePoints.Count;
        }
    }

    private void RewindTime()
    {
        if (timePoints.Count > 0)
        {
            TimePoint targetPoint = timePoints[0];
            
            // حرکت نرم به سمت نقطه هدف
            transform.position = Vector3.Lerp(transform.position, targetPoint.position, rewindSpeed * Time.fixedDeltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetPoint.rotation, rewindSpeed * Time.fixedDeltaTime);
            
            // اگر به اندازه کافی به نقطه هدف نزدیک شدیم، آن را حذف کن
            if (Vector3.Distance(transform.position, targetPoint.position) < positionThreshold)
            {
                lastRewindPoint = targetPoint;
                timePoints.RemoveAt(0);
                currentRewindTime -= recordInterval; // کاهش زمان واقعی هنگام برگشت
                recordedPoints = timePoints.Count;
            }
        }
        else
        {
            reachedEndOfRewind = true;
            // وقتی به انتهای مسیر برگشت رسیدیم، جسم را در همان جا نگه می‌داریم
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    public void StartRewind()
    {
        if (timePoints.Count > 0)
        {
            isRewinding = true;
            rb.isKinematic = true;
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            
            if (spriteRenderer != null)
                spriteRenderer.color = rewindColor;
        }
    }

    public void StopRewind()
    {
        isRewinding = false;
        rb.isKinematic = false;
        
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }
}

// اضافه کردن ویژگی ReadOnly برای نمایش فیلدها در Inspector
public class ReadOnlyAttribute : PropertyAttribute { }

#if UNITY_EDITOR
[UnityEditor.CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : UnityEditor.PropertyDrawer
{
    public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        UnityEditor.EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}
#endif

[System.Serializable]
public class TimePoint
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector2 velocity;
    public float angularVelocity;

    public TimePoint(Vector3 pos, Quaternion rot, Vector2 vel, float angVel)
    {
        position = pos;
        rotation = rot;
        velocity = vel;
        angularVelocity = angVel;
    }
}
