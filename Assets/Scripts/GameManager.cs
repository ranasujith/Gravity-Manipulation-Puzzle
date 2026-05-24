using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("CONTROLS")]
    public GameObject controlsPanel;
    public Button goBtn;

    [Header("UI")]
    public TMP_Text timerText;
    public TMP_Text cubeText;
    public TMP_Text messageText;

    [Header("GAME OVER")]
    public GameObject gameOverPanel;
    public Button homeButtonGO;
    public Button restartButtonGO;

    [Header("Win")]
    public GameObject winPanel;
    public Button homeButtonWin;
    public Button restartButtonWin;

    [Header("GAME")]
    public int totalCubes = 5;

    private int collectedCubes;

    public float timer = 120f;
    private bool gameStarted;
    private bool gameEnded;

    [Header("EXIT")]
    public GameObject exitpanel;
    public Button exitButton;
    public Button cancelButton;
    public Button homeButton;

    [Header("AUDIO CLIPS")]
    public AudioClip iconClickSFX;
    public AudioClip winSFX;
    public AudioClip gameOverSFX;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        controlsPanel.SetActive(true);

        Time.timeScale = 0f;

        UnlockCursor();

        goBtn.onClick.AddListener(StartGame);

        homeButtonGO.onClick.AddListener(HomeScene);
        homeButtonWin.onClick.AddListener(HomeScene);

        restartButtonGO.onClick.AddListener(RestartGame);
        restartButtonWin.onClick.AddListener(RestartGame);
        exitButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySFX(iconClickSFX);
            exitpanel.SetActive(true);

            UnlockCursor();
            Time.timeScale = 0f;

        });
        cancelButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySFX(iconClickSFX);
            Time.timeScale = 1f;

            exitpanel.SetActive(false);
        });
        homeButton.onClick.AddListener(HomeScene);
    }
    private void StartGame()
    {
        AudioManager.Instance.PlaySFX(iconClickSFX);
        controlsPanel.SetActive(false);

        gameStarted = true;

        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (gameEnded || !gameStarted)
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

        timerText.text = string.Format("{0:00}:{1:00}",minutes,seconds);

        cubeText.text = "Cubes: " + collectedCubes + " / " + totalCubes;
    }

    public void CollectCube()
    {
        collectedCubes++;

        if (collectedCubes >= totalCubes)
        {
            ShowMessage("All Cubes are collected!");
        }
    }
    public void ShowMessage(string message)
    {
        StartCoroutine(ShowCollectMessageRoutine(message));
    }

    private IEnumerator ShowCollectMessageRoutine(string message)
    {
        gameEnded = true;

        messageText.text = "All Cubes Collected!";

        yield return new WaitForSeconds(1f);

        messageText.text = string.Empty;

        WinGame();
    }
    public void WinGame()
    {
        gameEnded = true;
        AudioManager.Instance.PlaySFX(winSFX);
        winPanel.SetActive(true);
        AudioManager.Instance.StopMusic();
        Time.timeScale = 0f;

        UnlockCursor();
    }

    public void GameOver()
    {
        if (gameEnded)
            return;

        gameEnded = true;
        AudioManager.Instance.PlaySFX(gameOverSFX);
        gameOverPanel.SetActive(true);
        AudioManager.Instance.StopMusic();

        Time.timeScale = 0f;

        UnlockCursor();
    }

    public void RestartGame()
    {
        AudioManager.Instance.PlaySFX(iconClickSFX);
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public void HomeScene()
    {
        AudioManager.Instance.PlaySFX(iconClickSFX);
        Time.timeScale = 1f;

        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible = true;

        SceneManager.LoadScene("HomeScene");
    }

    private void UnlockCursor()
    {
        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible = true;
    }
}