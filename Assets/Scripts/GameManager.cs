using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UI")]
    public TMP_Text timerText;
    public TMP_Text cubeText;
    public GameObject gameOverPanel;
    public GameObject winPanel;

    [Header("GAME")]
    public int totalCubes = 5;

    private int collectedCubes;

    private float timer = 120f;

    private bool gameEnded;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (gameEnded)
            return;

        HandleTimer();

        //UpdateUI();
    }

    private void HandleTimer()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            timer = 0f;

            GameOver();
        }
    }

    private void UpdateUI()
    {
        int minutes = Mathf.FloorToInt(timer / 60f);

        int seconds = Mathf.FloorToInt(timer % 60f);

        timerText.text =
            string.Format("{0:00}:{1:00}",
            minutes,
            seconds);

        cubeText.text =
            "Cubes: " +
            collectedCubes +
            " / " +
            totalCubes;
    }

    public void CollectCube()
    {
        collectedCubes++;

        if (collectedCubes >= totalCubes)
        {
            WinGame();
        }
    }

    private void WinGame()
    {
        gameEnded = true;

        winPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void GameOver()
    {
        if (gameEnded)
            return;

        gameEnded = true;

        gameOverPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}