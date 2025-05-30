using System.Collections.Generic;
using UnityEngine;

public class ShadowGhost : MonoBehaviour
{
    [Header("تنظیمات سایه")]
    private float lifeTimer = 0f;
    
    public List<ShadowState> replayData;
    private int currentIndex = 0;

    public float moveSpeed = 10f;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            Color ghostColor = spriteRenderer.color;
            ghostColor.a = 0.4f;
            spriteRenderer.color = ghostColor;
        }
    }

    void Update()
    {
        if (replayData == null || replayData.Count == 0)
            return;
        
        if (currentIndex >= replayData.Count)
        {
            Destroy(gameObject);
            return;
        }

        ShadowState state = replayData[currentIndex];
        transform.position = Vector2.Lerp(transform.position, state.position, moveSpeed * Time.deltaTime);

        // اعمال انیمیشن‌ها
        if (animator != null)
        {
            animator.SetBool("isJumping", state.isJumping);
            animator.SetBool("isDashing", state.isDashing);
            animator.SetBool("IsWallSliding", state.isWallSliding);
            animator.SetBool("isRunning", state.isRunning);
        }

        // تنظیم جهت نگاه
        if ((state.isFacingRight && transform.localScale.x < 0) || (!state.isFacingRight && transform.localScale.x > 0))
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }

        currentIndex++;
    }

    public void SetReplay(List<ShadowState> data)
    {
        replayData = data;
        currentIndex = 0;
    }
    
    void StartDisappearSequence()
{

    if (animator != null)
    {
        animator.SetTrigger("Disappear"); // پخش انیمیشن محو
    }

    Destroy(gameObject, 0.6f); // حذف بعد از مدت انیمیشن
}
    public void MakeRespawnFinishedtrue()
    {
        // هیچ کاری نمی‌کنیم
    }
}