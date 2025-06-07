using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReturnToMainMenuButton()
    {
        // برگشت به منوی اصلی از طریق UIManager
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ExitToMainMenu();
        }
    }

    public void RestartGameButton()
    {
        // شروع مجدد بازی از طریق UIManager
        if (UIManager.Instance != null)
        {
            UIManager.Instance.RestartGame();
        }
    }
}
