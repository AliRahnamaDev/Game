using UnityEngine;
using Cinemachine;

public class Cameraa : MonoBehaviour
{
    [Header("Targets")]
    public Transform player;
    public Transform ground;

    [Header("Offset Settings")]
    public Vector2 defaultOffset = new Vector2(0f, 0f);     // حالت نرمال
    public Vector2 elevatedOffset = new Vector2(0f, -3f);   // حالت بالا رفتن

    [Header("Trigger Settings")]
    public float heightThreshold = 4f; // آستانه فاصله Y

    [Header("Smoothing")]
    public float smoothSpeed = 5f;

    private CinemachineVirtualCamera vcam;
    private CinemachineFramingTransposer transposer;

    private Vector2 currentOffset;
    private Vector2 targetOffset;

    void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        transposer = vcam.GetCinemachineComponent<CinemachineFramingTransposer>();

        if (transposer == null)
        {
            Debug.LogError("CinemachineFramingTransposer component not found. Set 'Body' to 'Framing Transposer' in your virtual camera.");
            enabled = false;
            return;
        }

        currentOffset = defaultOffset;
        targetOffset = defaultOffset;
    }

    void LateUpdate()
    {
        if (player == null || ground == null) return;

        float verticalDistance = player.position.y - ground.position.y;

        // تصمیم‌گیری درباره offset هدف
        if (verticalDistance > heightThreshold)
        {
            targetOffset = elevatedOffset;
        }
        else
        {
            targetOffset = defaultOffset;
        }

        // تغییر نرم offset فعلی به سمت offset هدف
        currentOffset = Vector2.Lerp(currentOffset, targetOffset, Time.deltaTime * smoothSpeed);

        // اعمال offset به Cinemachine
        transposer.m_TrackedObjectOffset = new Vector3(currentOffset.x, currentOffset.y, 0);
    }
}