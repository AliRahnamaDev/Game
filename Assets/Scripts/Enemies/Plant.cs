using UnityEngine;

public class Plant : Enemy
{
    [Header("Shooting Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float fireRate = 1.5f;
    private float fireCooldown;
    public bool isAttacking;

    [SerializeField] private bool playerDetection;
    [SerializeField] private float detectionRange = 5f;

    protected override void Update()
    {
        base.Update();
        HandleCollisions();

        fireCooldown -= Time.deltaTime;

        if (playerDetection && fireCooldown <= 0f)
        {
            Attack();
            fireCooldown = fireRate;
        }
        Animate();
    }

    private void Attack()
    {
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        Vector2 shootDir = new Vector2(facingDirection, 0); // فقط افقی شلیک می‌کنه
        rb.velocity = shootDir * bulletSpeed;
    }

    public override void HandleCollisions()
    {
        base.HandleCollisions();
        playerDetection = Physics2D.Raycast(transform.position, Vector2.right * facingDirection, detectionRange, whatIsPlayer);
    }

    public void Animate()
    {
        if (playerDetection)
        {
            isAttacking = true;
        }
        else
        {
            isAttacking = false;
        }
        animator.SetBool("isAttacking",isAttacking);
    }
}