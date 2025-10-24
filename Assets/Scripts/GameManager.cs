using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private int level;
    private int lives;
    private int scores;
    private bool isLoading = false;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        NewGame();
    }

    private void NewGame()
    {
        lives = 3;
        scores = 0;
        LoadLevel(1);
    }

    public void LevelFailed()
    {
        lives--;
        if (lives <= 0)
        {
            NewGame();
        }
        else
        {
            ReloadCurrentLevel();
        }
    }

    public void LevelComplete()
    {
        scores += 1000;

        int nextLevel = level + 1;

        if (nextLevel < SceneManager.sceneCountInBuildSettings)
        {
            LoadLevel(nextLevel);
        }
        else
        {
            LoadLevel(1);
        }
    }

    private void LoadLevel(int index)
    {
        if (isLoading) return;
        isLoading = true;
        StartCoroutine(LoadLevelAsync(index));
    }

    private IEnumerator LoadLevelAsync(int index)
    {
        if (SceneManager.sceneCount > 1)
        {
            Scene currentScene = SceneManager.GetSceneAt(1);
            yield return SceneManager.UnloadSceneAsync(currentScene);
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
        yield return asyncLoad;

        Scene newScene = SceneManager.GetSceneByBuildIndex(index);
        SceneManager.SetActiveScene(newScene);

        level = index;
        isLoading = false;
    }

    private void ReloadCurrentLevel()
    {
        LoadLevel(level);
    }
}