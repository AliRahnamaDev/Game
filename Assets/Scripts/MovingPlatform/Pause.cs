using System;
using System.Collections;
using UnityEngine;

public class Pause : MonoBehaviour
{
    [HideInInspector]
    public bool shouldPausePlatform = false;
    private Animator animator;
    public float stoptime = 3f;
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            shouldPausePlatform = true;
            animator.SetBool("isHitted",true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(PauseCoroutine());
            animator.SetBool("isHitted", false);
        }
    }

    private IEnumerator PauseCoroutine()
    {
        yield return new WaitForSeconds(stoptime);
        shouldPausePlatform = false;
    }
}