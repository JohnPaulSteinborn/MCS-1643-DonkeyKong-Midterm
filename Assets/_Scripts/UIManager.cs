using System.Collections;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject pauseMenuPanel;
    public GameObject gameOverPanel;

    private bool isPaused = false;

    private static UIManager instance;

    private void Awake()
    {
        // Prevent duplicates
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
        ShowMainMenu();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void ShowGameOver()
    {
        mainMenuPanel.SetActive(false);
        pauseMenuPanel.SetActive(false);
        gameOverPanel.SetActive(true);

        Time.timeScale = 0f;

        // Stop music on Game Over
        FindObjectOfType<MusicManager>()?.StopMusic();
    }

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        pauseMenuPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        Time.timeScale = 0f;
    }

    public void StartGame()
    {
        mainMenuPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        Time.timeScale = 1f;

        // Start background music
        FindObjectOfType<MusicManager>()?.PlayMusic();

        // Start the game logic
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
            gm.StartNewGame();
        else
            Debug.LogWarning("GameManager not found!");
    }

    public void PauseGame()
    {
        isPaused = true;
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void RetryGame()
    {
        gameOverPanel.SetActive(false);
        Time.timeScale = 1f;
        FindObjectOfType<GameManager>()?.RestartGame();
        FindObjectOfType<MusicManager>()?.PlayMusic();
    }

    public void ExitToMainMenu()
    {
        gameOverPanel.SetActive(false);
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;

        FindObjectOfType<GameManager>()?.ReturnToMainMenu();

        ShowMainMenu();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
