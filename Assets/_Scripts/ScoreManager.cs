using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public TextMeshProUGUI scoreText;
    private static int score;

    private void Awake()
    {
        // Singleton setup so only one ScoreManager exists across scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        UpdateScoreUI();
    }

    public static void AddScore(int points)
    {
        score += points;
        if (Instance != null)
        {
            Instance.UpdateScoreUI();
        }
    }

    public static void ResetScore()
    {
        score = 0;
        if (Instance != null)
        {
            Instance.UpdateScoreUI();
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    public int GetScore()
    {
        return score;
    }
}
