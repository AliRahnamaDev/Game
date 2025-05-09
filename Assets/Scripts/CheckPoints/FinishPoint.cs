using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishPoint : MonoBehaviour
{
    private Animator animator => GetComponent<Animator>();
    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            animator.SetTrigger("isActivated");
            Debug.Log("LevalFinished");
        }
    }
}
