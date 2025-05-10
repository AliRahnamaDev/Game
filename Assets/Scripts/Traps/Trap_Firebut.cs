using System;
using UnityEngine;

public class Trap_Firebut : MonoBehaviour
{
    private BoxCollider2D col;//collider bayad jabeaj she
    private Animator animator;
    private Trap_Fire trap_Fire;
    private bool playerIsInside = false;
    private float timeExited = -1f;
    public float TimeUntilActivateFire = 3f;

    private void Start()
    {
        col = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        trap_Fire = GameObject.Find("Fire").GetComponent<Trap_Fire>();
    }

    private void Update()
    {
        // اگر بازیکن بیرون است و 3 ثانیه گذشته:
        if (!playerIsInside && timeExited > 0 && Time.time - timeExited >= TimeUntilActivateFire)
        {
            trap_Fire.OnAnimation();
            timeExited = -1f; // فقط یک‌بار فعال شود
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            col.offset = new Vector2(col.offset.x, col.offset.y - 0.1f);
            animator.SetBool("isHitted",true);
            playerIsInside = true;
            trap_Fire.OffAnimation();
            timeExited = -1f; // خروج قبلی بی‌اثر شود
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetBool("isHitted",false);
            playerIsInside = false;
            timeExited = Time.time; // زمان خروج را ذخیره کن
        }
    }
}