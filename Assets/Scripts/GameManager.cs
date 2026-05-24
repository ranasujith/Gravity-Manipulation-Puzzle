using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public TMP_Text timerText;
    public TMP_Text cubeText;
    public GameObject gameOverPanel;
    public GameObject winPanel;
    public TMP_Text warningText;

    [Header("GAME")]
    public int totalCubes = 5;

    private int collectedCubes;

    private float timer = 120f;

    private bool gameEnded;

    [Header("EXIT")]
    public GameObject exitpanel;
    public Button exitButton;
    public Button cancelButton;
    public Button homeButton;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        exitButton.onClick.AddListener(() =>
        {
            Time.timeScale = 0f;
            exitpanel.SetActive(true);

        });
        cancelButton.onClick.AddListener(() => 
        {
            Time.timeScale = 0f;
            exitpanel.SetActive(false);
        });
        homeButton.onClick.AddListener(HomeScene);
    }

    private void Update()
    {
        if (gameEnded)
            return;

        HandleTimer();

        UpdateUI();
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

    }
    public bool HasCollectedAllCubes()
    {
        return collectedCubes >= totalCubes;
    }
    public void ShowCollectAllCubesMessage()
    {
        StartCoroutine(
            ShowCollectMessageRoutine()
        );
    }

    private IEnumerator ShowCollectMessageRoutine()
    {
        warningText.text = "Collect all the cubes";

        yield return new WaitForSeconds(2f);

        warningText.text = string.Empty;
    }
    public void WinGame()
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

    public void HomeScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("HomeScene");
    }
}