using UnityEngine;

public class Tigh : MonoBehaviour
{
    [Header("Laser Settings")]
    [SerializeField] private Transform laserStartPoint;
    [SerializeField] private LineRenderer laserLine;
    [SerializeField] private float laserMaxDistance = 100f;
    [SerializeField] private LayerMask laserHitLayers;
    [SerializeField] private float lineWidth = 0.03f; // باریک

    [Header("Shooting Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.2f;

    private Camera mainCamera;
    private bool isLaserActive = false;
    private Vector3 hitPoint;
    private float currentAngle;
    private float nextFireTime = 0f;

    void Start()
    {
        mainCamera = Camera.main;

        if (laserStartPoint == null || laserLine == null || firePoint == null || bulletPrefab == null)
        {
            Debug.LogError("یکی از اجزای لازم مقداردهی نشده!");
            return;
        }

        // تنظیمات LineRenderer: رنگ خاکستری کم‌رنگ، عرض باریک
        laserLine.positionCount = 2;
        laserLine.startWidth = lineWidth;
        laserLine.endWidth = lineWidth;
        laserLine.useWorldSpace = true;

        Color faintGray = new Color(0.7f, 0.7f, 0.7f, 0.3f);
        laserLine.startColor = faintGray;
        laserLine.endColor = faintGray;

        // برای نقطه‌چین: از متریال با بافت dashed استفاده شود (از Inspector)
        laserLine.textureMode = LineTextureMode.Tile;
    }

    void Update()
    {
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;
        Vector3 direction = mousePosition - laserStartPoint.position;
        currentAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        isLaserActive = Input.GetMouseButton(1);
        laserLine.enabled = isLaserActive;

        if (isLaserActive)
            UpdateLaser();

        if (Input.GetMouseButton(0) && isLaserActive && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void UpdateLaser()
    {
        if (laserLine == null || laserStartPoint == null) return;

        Vector3 laserStart = laserStartPoint.position;
        laserStart.z = 0f;

        Vector3 laserDirection = new Vector3(
            Mathf.Cos(currentAngle * Mathf.Deg2Rad),
            Mathf.Sin(currentAngle * Mathf.Deg2Rad),
            0f);

        RaycastHit2D hit = Physics2D.Raycast(laserStart, laserDirection, laserMaxDistance, laserHitLayers);

        Vector3 laserEnd = hit.collider != null
            ? hit.point
            : laserStart + laserDirection * laserMaxDistance;

        laserEnd.z = 0f;

        laserLine.SetPosition(0, laserStart);
        laserLine.SetPosition(1, laserEnd);
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0, 0, currentAngle));
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

        // اضافه کردن کامپوننت چرخش به گلوله

        if (bulletRb != null)
        {
            Vector2 shootDirection = new Vector2(
                Mathf.Cos(currentAngle * Mathf.Deg2Rad),
                Mathf.Sin(currentAngle * Mathf.Deg2Rad));
            bulletRb.velocity = shootDirection * bulletSpeed;
        }
        else
        {
            Debug.LogError("گلوله Rigidbody2D ندارد!");
        }

        Destroy(bullet, 5f);
    }
}
