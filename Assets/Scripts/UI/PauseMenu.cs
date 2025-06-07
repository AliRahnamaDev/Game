using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResumeGame()
    {
        // ادامه بازی از طریق UIManager
        if (UIManager.Instance != null)
        {
            Debug.Log("Resume button clicked");
            UIManager.Instance.ResumeGame();
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

    public void OpenSettings()
    {
        // نمایش منوی تنظیمات از طریق UIManager
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowSettingsMenu();
        }
    }

    public void ExitToMainMenuButton()
    {
        // خروج به منوی اصلی از طریق UIManager
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ExitToMainMenu();
        }
    }
}
