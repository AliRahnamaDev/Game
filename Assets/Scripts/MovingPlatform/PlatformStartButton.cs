using UnityEngine;

public class PlatformStartButton : MonoBehaviour
{
    private Animator animator;
    public bool isPressed = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetBool("isHitted", true);
            isPressed = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetBool("isHitted", false);
            isPressed = false;
        }
    }
}

