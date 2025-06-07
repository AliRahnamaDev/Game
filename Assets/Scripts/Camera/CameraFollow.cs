using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    private Transform _parent; // فقط مقداردهی در زمان اجرا
    [SerializeField] private Vector3 _offset = new Vector3(0, 0, -10);
    [SerializeField, Range(0.01f, 1f)] private float _smoothness = 0.1f;

    [Header("Camera Bounds")]
    [SerializeField] private Vector2 minPosition;
    [SerializeField] private Vector2 maxPosition;

    private Transform _currentTarget;

    /// <summary>
    /// دریافت پدر واقعی فرم‌ها (realPlayer)
    /// </summary>
    public void SetTarget(Transform parentOfForms)
    {
        _parent = parentOfForms;
        _currentTarget = GetActiveChild(_parent);
    }

    private void LateUpdate()
    {
        if (_parent == null) return;

        // پیدا کردن فرزند فعال در هر فریم
        Transform newTarget = GetActiveChild(_parent);
        if (newTarget != null)
        {
            _currentTarget = newTarget;
        }

        if (_currentTarget == null) return;

        Vector3 targetPosition = _currentTarget.position + _offset;
        targetPosition.x = Mathf.Clamp(targetPosition.x, minPosition.x, maxPosition.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minPosition.y, maxPosition.y);

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            _smoothness * Time.deltaTime * 60f
        );
    }

    private Transform GetActiveChild(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.gameObject.activeSelf)
                return child;
        }
        return null;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 bottomLeft = new Vector3(minPosition.x, minPosition.y, 0);
        Vector3 topRight = new Vector3(maxPosition.x, maxPosition.y, 0);

        Gizmos.DrawLine(bottomLeft, new Vector3(maxPosition.x, minPosition.y, 0));
        Gizmos.DrawLine(bottomLeft, new Vector3(minPosition.x, maxPosition.y, 0));
        Gizmos.DrawLine(topRight, new Vector3(minPosition.x, maxPosition.y, 0));
        Gizmos.DrawLine(topRight, new Vector3(maxPosition.x, minPosition.y, 0));
    }
#endif
}

