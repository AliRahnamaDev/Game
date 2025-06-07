using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Canvases / Panels")]
    public GameObject mainMenu;
    public GameObject pauseMenu;
    public GameObject gameOverMenu;
    public GameObject settingsMenu;

    private void Awake()
    {
        // Singleton implementation
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Called automatically when a new scene is loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene loaded: " + scene.name);
        StartCoroutine(RebindMenusWithDelay());
    }

    private IEnumerator RebindMenusWithDelay()
    {
        // Wait for scene UI to be fully instantiated
        yield return new WaitForSeconds(0.05f);
        RebindMenus();
    }

    // Reconnects menu panel references after scene change
    public void RebindMenus()
    {
        Canvas mainCanvas = FindObjectOfType<Canvas>();

        if (mainCanvas == null)
        {
            Debug.LogWarning("UIManager: Canvas not found!");
            return;
        }

        Transform canvasTransform = mainCanvas.transform;

        mainMenu = canvasTransform.Find("MainMenuPanel")?.gameObject;
        pauseMenu = canvasTransform.Find("PauseMenuPanel")?.gameObject;
        gameOverMenu = canvasTransform.Find("GameOverPanel")?.gameObject;
        settingsMenu = canvasTransform.Find("SettingsPanel")?.gameObject;

        Debug.Log("UIManager: Menus rebound.");
    }

    public void CloseAllMenus()
    {
        if (mainMenu != null) mainMenu.SetActive(false);
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (gameOverMenu != null) gameOverMenu.SetActive(false);
        if (settingsMenu != null) settingsMenu.SetActive(false);
    }

    public void ShowMainMenu()
    {
        CloseAllMenus();
        if (mainMenu != null) mainMenu.SetActive(true);
    }

    public void ShowPauseMenu()
    {
        CloseAllMenus();

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
            Debug.Log("PauseMenu activated");
        }
        else
        {
            Debug.LogWarning("pauseMenu is null!");
        }
    }

    public void ShowGameOverMenu()
    {
        CloseAllMenus();

        if (gameOverMenu != null)
        {
            gameOverMenu.SetActive(true);
            Time.timeScale = 0;
            Debug.Log("GameOverMenu activated");
        }
        else
        {
            Debug.LogWarning("gameOverMenu is null!");
        }
    }

    public void ShowSettingsMenu()
    {
        if (settingsMenu != null) settingsMenu.SetActive(true);
    }

    public void CloseSettingsMenu()
    {
        if (settingsMenu != null) settingsMenu.SetActive(false);
    }

    public void ResumeGame()
    {
        CloseAllMenus();
        Time.timeScale = 1f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        StartCoroutine(RestartRoutine());
        CloseAllMenus();
    }

    private IEnumerator RestartRoutine()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        SceneManager.LoadScene(currentSceneIndex);
        yield return null; // wait 1 frame
        RebindMenus();
    }

  public void ExitToMainMenu()
{
    Time.timeScale = 1;
    StartCoroutine(LoadMainMenuScene());
}

private IEnumerator LoadMainMenuScene()
{
    SceneManager.LoadScene(0); // Main Menu scene
    yield return null;

    RebindMenus(); // ← رفرنس‌ها رو مجدد بگیر

    ShowMainMenu(); // ← منوی اصلی رو نمایش بده
}


    public void ExitGame()
    {
        Debug.Log("Game is exiting...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
