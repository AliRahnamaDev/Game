using UnityEngine;

public class WayPointFollower : MonoBehaviour
{
    [Header("Waypoints Parent (as child of this object)")]
    public Transform waypointParent;

    [Header("Movement Settings")]
    public float speed = 2f;
    public bool loop = true;

    private int currentIndex = 0;
    private Vector3[] waypointWorldPositions;

    void Start()
    {
        if (waypointParent == null || waypointParent.childCount == 0)
        {
            Debug.LogError("No waypoints assigned!");
            return;
        }

        // ذخیره موقعیت جهانی تمام waypointها
        waypointWorldPositions = new Vector3[waypointParent.childCount];
        for (int i = 0; i < waypointParent.childCount; i++)
        {
            waypointWorldPositions[i] = waypointParent.GetChild(i).position;
        }

        // حالا از parent جدا کن تا با object اصلی حرکت نکنند
        waypointParent.SetParent(null);
    }

    void Update()
    {
        if (waypointWorldPositions == null || waypointWorldPositions.Length == 0) return;

        Vector3 target = waypointWorldPositions[currentIndex];
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.05f)
        {
            currentIndex++;
            if (currentIndex >= waypointWorldPositions.Length)
            {
                if (loop)
                    currentIndex = 0;
                else
                    enabled = false;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (waypointParent == null || waypointParent.childCount < 2) return;

        Gizmos.color = Color.green;
        for (int i = 0; i < waypointParent.childCount - 1; i++)
        {
            Gizmos.DrawLine(waypointParent.GetChild(i).position, waypointParent.GetChild(i + 1).position);
        }

        if (loop)
        {
            Gizmos.DrawLine(waypointParent.GetChild(waypointParent.childCount - 1).position, waypointParent.GetChild(0).position);
        }
    }
}

