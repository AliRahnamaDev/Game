using UnityEngine;

public class PauseHandler : MonoBehaviour
{
    void Update()
    {
        if (!UIManager.Instance) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // اگر GameOver بازه، هیچی نکن (یا ببندش اگه خواستی)
            if (UIManager.Instance.gameOverMenu != null && UIManager.Instance.gameOverMenu.activeSelf)
                return;

            // اگر Settings بازه، فقط اون رو ببند
            if (UIManager.Instance.settingsMenu != null && UIManager.Instance.settingsMenu.activeSelf)
            {
                UIManager.Instance.CloseSettingsMenu();
                return;
            }

            // اگر PauseMenu بازه → ببند
            if (UIManager.Instance.pauseMenu != null && UIManager.Instance.pauseMenu.activeSelf)
            {
                UIManager.Instance.ResumeGame();
                return;
            }

            // در غیر این صورت → نشون بده
            UIManager.Instance.ShowPauseMenu();
        }
    }
}

