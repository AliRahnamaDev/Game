using System.Collections;
using UnityEngine;

public class Enemy_bird :MonoBehaviour
{
    private BoxCollider2D collider;
    private Rigidbody2D rb2d;
    private Animator animator;
    [SerializeField] protected GameObject damageTrigger;
   
    [Header("BirdDetails")]
    public bool isDead = false;
    public float deathRoatationSpeed = 300;
    public float speed;
    public float waitTime=1;
    public float flyForce;
    public bool canFlip=true;
    public float deathImpact = 5;
    public Transform start;
    public Transform end;
    
    public bool isFacingRight = false;
    public int FacingDirection = -1;
    public float yOffset;
    public float minY;
    void Start()
    {
        end.transform.position = new Vector2(transform.position.x ,start.transform.position.y);
        transform.position = new Vector2(start.transform.position.x, transform.position.y);
        minY = transform.position.y - yOffset;
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        collider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (isDead)
        {
            transform.Rotate(0, 0, deathRoatationSpeed * Time.deltaTime);
            return;
        }

        if (transform.position.x >= end.transform.position.x || transform.position.x <= start.transform.position.x)
        {
            StartCoroutine(Flip());
        }

        HandleMovement();

        if (transform.position.y < minY)
        {
            rb2d.AddForce(Vector2.up * flyForce);
        }
    }


    public void HandleMovement()
    {
        rb2d.velocity = new Vector2(speed*FacingDirection, rb2d.velocity.y);
    }
    IEnumerator Flip()
    {
        if (canFlip)
        {
            FacingDirection *= -1;
            transform.Rotate(0f, 180f, 0f);
            isFacingRight = !isFacingRight;
            canFlip = false;
            yield return new WaitForSeconds(waitTime);
            canFlip = true;
        }
    }

    public void Die()
    {
        isDead = true;
        damageTrigger.gameObject.SetActive(false);
        rb2d.velocity = new Vector2(rb2d.velocity.x, deathImpact);
        collider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            float playerY = other.transform.position.y;
            float enemyY = transform.position.y;

            if (playerY > enemyY) // پلیر از بالا آمده
            {
                Die();
            }
        }
    }
}