using UnityEngine;

public class Fan : MonoBehaviour
{
    [Header("Fan Settings")]
    public bool isAlwaysOn = false;        // آیا همیشه روشن باشد؟
    public bool isInitiallyOn = true;      // وضعیت اولیه
     public bool isOn = false; // وضعیت فعلی فن
     public float xwide;
    [Header("Wind Settings")]
    public float windForce = 10f;          // قدرت باد
    public float windRange = 5f;           // برد باد
    public LayerMask affectedLayers;       // لایه‌هایی که باد روی آن‌ها اثر دارد

    private Vector2 windDirection;
    private Animator animator;
    public ParticleSystem windParticles;


    private void Start()
    {
        animator = GetComponent<Animator>();
        // تنظیم جهت باد بر اساس چرخش فن (فرض: اسپریت به سمت بالا است)
        windDirection = transform.up.normalized;

        // تنظیم وضعیت اولیه فن
        if (isAlwaysOn)
        {
            isOn = true;
        }
        else
        {
            isOn = isInitiallyOn;
        }
    }

    private void FixedUpdate()
    {
        AnimateFan();
        if (!isOn) return;

        // ناحیه‌ای که باد در آن اعمال می‌شود (BoxCast)
        Vector2 boxCenter = (Vector2)transform.position + windDirection * windRange / 2f;
        Vector2 boxSize = new Vector2(xwide, windRange); // عرض باد وابسته به Scale.x

        // چرخش با توجه به جهت فن
        float angle = Mathf.Atan2(windDirection.y, windDirection.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle - 90); // برای align شدن با up

        // همه اشیایی که در مسیر باد هستند را پیدا کن
        Collider2D[] hits = Physics2D.OverlapBoxAll(boxCenter, boxSize, angle - 90, affectedLayers);
        foreach (var hit in hits)
        {
            Rigidbody2D rb = hit.attachedRigidbody;
            if (rb != null)
            {
                rb.AddForce(windDirection * windForce * Time.fixedDeltaTime, ForceMode2D.Force);
            }
        }
    }

    public void SetFanState(bool state)
    {
        if (!isAlwaysOn)
            isOn = state;
    }

    private void AnimateFan()
    {
        if (animator == null) return;

        animator.SetBool("isActive", isOn);
        if (isOn)
        {
            windParticles.Play();
        }
        else
        {
            windParticles.Stop();
        }
    }


    public void ToggleFan()
    {
        if (!isAlwaysOn)
            isOn = !isOn;
    }

    private void OnDrawGizmosSelected()
    {
        // نمایش محدوده باد در Scene
        Gizmos.color = Color.cyan;
        Vector2 dir = Application.isPlaying ? windDirection : (Vector2)transform.up;
        Vector2 boxCenter = (Vector2)transform.position + dir * windRange / 2f;
        Vector2 boxSize = new Vector2(xwide, windRange);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(boxCenter, Quaternion.Euler(0, 0, angle - 90), Vector3.one);
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
    }
}
