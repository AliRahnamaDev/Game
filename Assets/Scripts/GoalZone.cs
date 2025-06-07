using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalZone : MonoBehaviour
{
    private bool player1Reached = false;
    private bool player2Reached = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            player1Reached = true;

        if (player1Reached)
        {
            SceneManager.LoadScene(2); // یا اسم Scene: SceneManager.LoadScene("WinScene");
        }
    }
}