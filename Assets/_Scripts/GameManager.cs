using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private int level;
    private int lives;
    private bool isLoading = false;

    private static GameManager instance;

    private void Awake()
    {
        // Prevent duplicate managers
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        NewGame();
    }

    private void NewGame()
    {
        lives = 3;
        ScoreManager.ResetScore();
        LoadLevel(1);
    }

    public void StartNewGame()
    {
        lives = 3;
        ScoreManager.ResetScore();
        LoadLevel(1);
    }

    public void LevelFailed()
    {
        lives--;

        if (lives <= 0)
        {
            UIManager ui = FindObjectOfType<UIManager>();
            if (ui != null)
                ui.ShowGameOver();
        }
        else
        {
            ReloadCurrentLevel();
        }
    }

    public void LevelComplete()
    {
        ScoreManager.AddScore(1000);

        int nextLevel = level + 1;

        if (nextLevel < SceneManager.sceneCountInBuildSettings)
            LoadLevel(nextLevel);
        else
            LoadLevel(1);
    }

    private void LoadLevel(int index)
    {
        if (isLoading) return;
        isLoading = true;
        StartCoroutine(LoadLevelAsync(index));
    }

    private IEnumerator LoadLevelAsync(int index)
    {
        // Unload current gameplay scene if one is loaded
        if (SceneManager.sceneCount > 1)
        {
            Scene currentScene = SceneManager.GetSceneAt(1);
            yield return SceneManager.UnloadSceneAsync(currentScene);
        }

        // Load new level additively
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
        yield return asyncLoad;

        Scene newScene = SceneManager.GetSceneByBuildIndex(index);
        SceneManager.SetActiveScene(newScene);

        level = index;
        isLoading = false;
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;

        // Stop music for menu
        FindObjectOfType<MusicManager>()?.StopMusic();

        if (SceneManager.sceneCount > 1)
        {
            Scene gameplayScene = SceneManager.GetSceneAt(1);
            if (gameplayScene.isLoaded)
                SceneManager.UnloadSceneAsync(gameplayScene);
        }

        lives = 3;
        ScoreManager.ResetScore();
    }

    private void ReloadCurrentLevel()
    {
        LoadLevel(level);
    }

    public void RestartGame()
    {
        lives = 3;
        ScoreManager.ResetScore();
        LoadLevel(1);
    }
}
