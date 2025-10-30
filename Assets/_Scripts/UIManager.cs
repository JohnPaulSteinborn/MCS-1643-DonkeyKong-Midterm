using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject pauseMenuPanel;
    public GameObject gameOverPanel;

    private bool isPaused = false;
    private void Start()
    {
        ShowMainMenu();
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    public void ShowGameOver()
    {
        mainMenuPanel.SetActive(false);
        pauseMenuPanel.SetActive(false);

        gameOverPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 0f;
    }
    public void StartGame()
    {
        mainMenuPanel.SetActive(false);
        Time.timeScale = 1f;

        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
        {
            gm.StartNewGame();
        }
        else
        {
            Debug.LogWarning("GameManager not found!");
        }
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
        FindObjectOfType<GameManager>().RestartGame();
    }
    public void ExitToMainMenu()
    {
        gameOverPanel.SetActive(false);
        pauseMenuPanel.SetActive(false);

        FindObjectOfType<GameManager>().ReturnToMainMenu();

        ShowMainMenu();
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
