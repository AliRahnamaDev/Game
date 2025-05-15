using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour
{
    
    public Transform[] waypointTransforms;
    public float moveSpeed = 2f;
    public float waitTime = 2f; // مدت مکث در ابتدا و انتها (بر حسب ثانیه)

    private Vector3[] waypoints;
    private int currentIndex = 0;
    private bool forward = true;
    private bool isWaiting = false;
    
    private Animator animator;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        // ذخیره موقعیت جهانی waypointها
        waypoints = new Vector3[waypointTransforms.Length];
        for (int i = 0; i < waypointTransforms.Length; i++)
        {
            waypoints[i] = waypointTransforms[i].position;
        }
    }

    void Update()
    {
        if (isWaiting || waypoints.Length == 0)
        {
            animator.SetBool("isActive", false); // پلتفرم منتظر است
            return;
        }

        Vector3 targetPos = waypoints[currentIndex];

        // چک کن که آیا واقعاً در حال حرکت هست یا نه
        bool isMoving = Vector3.Distance(transform.position, targetPos) > 0.01f;
        animator.SetBool("isActive", isMoving);

        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (!isMoving)
        {
            StartCoroutine(WaitAndMoveNext());
        }
    }

    
    IEnumerator WaitAndMoveNext()
    {
        isWaiting = true;

        // در ابتدا یا انتهای مسیر مکث کن
        if (currentIndex == 0 || currentIndex == waypoints.Length - 1)
        {
            yield return new WaitForSeconds(waitTime);
        }

        // تعیین waypoint بعدی
        if (forward)
        {
            currentIndex++;
            if (currentIndex >= waypoints.Length)
            {
                currentIndex = waypoints.Length - 2;
                forward = false;
            }
        }
        else
        {
            currentIndex--;
            if (currentIndex < 0)
            {
                currentIndex = 1;
                forward = true;
            }
        }

        isWaiting = false;
    }
}