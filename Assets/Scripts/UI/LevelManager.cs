using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Scene Order")]
    public string[] sceneNames; // مثلا: Stage1, Stage2, Stage3

    private int currentIndex;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        currentIndex = System.Array.IndexOf(sceneNames, currentScene);
    }

    public void LoadNextLevel()
    {
        if (currentIndex + 1 < sceneNames.Length)
        {
            SceneManager.LoadScene(sceneNames[++currentIndex]);
        }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(sceneNames[currentIndex]);
    }

    public void LoadLevelByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        currentIndex = System.Array.IndexOf(sceneNames, sceneName);
    }
}