using UnityEngine;

public class CeilingGravity : MonoBehaviour
{
    private Player player;
    private Rigidbody2D rb;
    private bool isOnCeiling = false;
    private float originalGravityScale;
    private bool isFacingRight = true;

    void Start()
    {
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
        originalGravityScale = rb.gravityScale;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleCeilingGravity();
        }

        if (isOnCeiling)
        {
            // چرخاندن کاراکتر به حالت وارونه
            transform.rotation = Quaternion.Euler(0, 0, 180);
        }
        else
        {
            // برگرداندن کاراکتر به حالت عادی
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    void ToggleCeilingGravity()
    {
        isOnCeiling = !isOnCeiling;
        
        if (isOnCeiling)
        {
            // اعمال گرانش معکوس
            rb.gravityScale = -originalGravityScale;
        }
        else
        {
            // برگرداندن گرانش به حالت عادی
            rb.gravityScale = originalGravityScale;
        }
    }
} 