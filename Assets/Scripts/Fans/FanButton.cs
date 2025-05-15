using System;
using UnityEngine;

public class FanButton : MonoBehaviour
{
    public enum ButtonType { Hold, Toggle }
    public ButtonType buttonType = ButtonType.Hold;

    [Header("Connected Fan")]
    public Fan connectedFan;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        animator.SetBool("isHitted", true);
        if (connectedFan == null)
        {
            Debug.LogWarning("ConnectedFan is not assigned to button!");
            return;
        }
        
        if (buttonType == ButtonType.Toggle)
        {
            connectedFan.ToggleFan();
        }
        else if (buttonType == ButtonType.Hold)
        {
            connectedFan.SetFanState(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (connectedFan == null) return;

        animator.SetBool("isHitted",false);
        if (buttonType == ButtonType.Hold)
        {
            connectedFan.SetFanState(false);
        }
        
    }
}