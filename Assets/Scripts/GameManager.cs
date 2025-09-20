using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public TMP_Text timerText;
    public TMP_Text scoreText;
    public TMP_Text mistakesText; // NEW: display mistakes during gameplay
    public GameObject endPanel;
    public TMP_Text endText;
    public TMP_Text endMistakesText; // NEW: display mistakes on end panel
    public Button replayButton;

    [Header("Intro UI")]
    public GameObject introPanel;
    public Button startButton;

    [Header("Fires")]
    public Fire[] fires;

    [Header("Settings")]
    public float gameTime = 30f;

    [Header("Effects")]
    public Image redFlashOverlay;

    private int score = 0;
    private int mistakes = 0; // NEW: track mistakes
    private float timeRemaining;
    public bool gameEnded = false;
    public bool gameStarted = false;

    private Coroutine fireRoutine;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        timeRemaining = gameTime;
        scoreText.text = "Score: 0";
        mistakesText.text = "Mistakes: 0"; // initialize
        endPanel.SetActive(false);

        if (introPanel != null)
        {
            introPanel.SetActive(true);
            Time.timeScale = 0f;
        }

        if (startButton != null)
            startButton.onClick.AddListener(StartGame);

        if (replayButton != null)
            replayButton.onClick.AddListener(ReplayGame);

        if (redFlashOverlay != null)
            redFlashOverlay.color = new Color(1, 0, 0, 0);
    }

    void Update()
    {
        // Press space to start the game (intro)
        if (!gameStarted && Input.GetKeyDown(KeyCode.Space))
            StartGame();

        // Press space to restart game (end panel)
        if (gameEnded && Input.GetKeyDown(KeyCode.Space))
            ReplayGame();

        if (!gameStarted || gameEnded) return;

        timeRemaining -= Time.deltaTime;
        timerText.text = "Time: " + Mathf.CeilToInt(timeRemaining);

        if (timeRemaining <= 0)
            EndGame();
    }

    public void StartGame()
    {
        gameStarted = true;
        introPanel.SetActive(false);
        endPanel.SetActive(false);
        Time.timeScale = 1f;

        if (fireRoutine == null)
            fireRoutine = StartCoroutine(RandomlyIgnite());
    }

    IEnumerator RandomlyIgnite()
    {
        while (!gameEnded)
        {
            yield return new WaitForSeconds(Random.Range(1f, 2f));
            Fire f = fires[Random.Range(0, fires.Length)];
            if (f.currentType == Fire.FireType.None)
            {
                Fire.FireType t = (Random.value > 0.5f) ? Fire.FireType.TypeA : Fire.FireType.TypeB;
                f.SetFire(t);
            }
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        scoreText.text = "Score: " + score;
    }

    public void WrongChoice(Fire fire)
    {
        // Increment mistake counter
        mistakes++;
        if (mistakesText != null)
            mistakesText.text = "Mistakes: " + mistakes;

        // Remove fire
        fire.SetFire(Fire.FireType.None);

        // Flash screen red
        if (redFlashOverlay != null)
            StartCoroutine(FlashRed());
    }

    IEnumerator FlashRed()
    {
        redFlashOverlay.color = new Color(1, 0, 0, 0.5f);
        yield return new WaitForSeconds(0.25f);
        redFlashOverlay.color = new Color(1, 0, 0, 0f);
    }

    public void EndGame()
    {
        gameEnded = true;
        endPanel.SetActive(true);
        endText.text = "Final Score: " + score;
        if (endMistakesText != null)
            endMistakesText.text = "Mistakes: " + mistakes;
        Time.timeScale = 0f;
    }

    public void ReplayGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
