using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        // بستن Main Menu
        if (UIManager.Instance != null)
        {
            UIManager.Instance.CloseAllMenus();
        }

        // لود صحنه بازی (مثلاً Scene 1)
        SceneManager.LoadScene(1);
    }


    public void OpenSettings()
    {
        UIManager.Instance.ShowSettingsMenu();
    }

    public void ExitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        Debug.Log("ExitGame called in Editor");
#endif
    }
}