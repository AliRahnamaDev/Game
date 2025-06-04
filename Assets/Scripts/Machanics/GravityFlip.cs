using UnityEngine;

[RequireComponent(typeof(Player))]
public class GravityFlip : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private KeyCode flipKey = KeyCode.M;
    [SerializeField] private float normalGravity = 4.5f;
    [SerializeField] private float flippedGravity = -4.5f;
    
    private Player player;
    private Rigidbody2D rb;
    private bool isGravityFlipped = false;
    
    private void Awake()
    {
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(flipKey) && player.canBeControlled && player.allowGravityInvert)
        {
            ToggleGravity();
        }
    }

    public void ToggleGravity()
    {
        isGravityFlipped = !isGravityFlipped;
        
        // اعمال گرانش جدید
        rb.gravityScale = isGravityFlipped ? flippedGravity : normalGravity;
        
        // چرخش کاراکتر
        transform.rotation = Quaternion.Euler(0, 0, isGravityFlipped ? 180f : 0f);
        
        // به روزرسانی جهت نگاه کاراکتر
        UpdatePlayerFacing();
    }

    private void UpdatePlayerFacing()
    {
        // حفظ جهت اصلی هنگام چرخش گرانش
        if ((isGravityFlipped && player.isFacingRight) || 
            (!isGravityFlipped && !player.isFacingRight))
        {
            player.Flip();
        }
    }
}