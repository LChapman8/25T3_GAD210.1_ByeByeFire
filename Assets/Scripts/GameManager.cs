using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public TMP_Text timerText;
    public TMP_Text scoreText;
    public GameObject endPanel;
    public TMP_Text endText;

    [Header("Intro UI")]
    public GameObject introPanel;  
    public TMP_Text introText;
    public Button startButton;

    [Header("Fires")]
    public Fire[] fires;

    [Header("Settings")]
    public float gameTime = 30f;

    private int score = 0;
    private float timeRemaining;
    private bool gameEnded = false;
    private bool gameStarted = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        timeRemaining = gameTime;
        scoreText.text = "Score: 0";
        endPanel.SetActive(false);

        
        if (introPanel != null)
        {
            introPanel.SetActive(true);
            Time.timeScale = 0f; 
        }

        if (startButton != null)
            startButton.onClick.AddListener(StartGame);

        if (introText != null)
        {
            introText.text =
                "Welcome to Bye Bye Fire!\n\n" +
                "Instructions:\n" +
                "- CO? = LMB (Red Fires)\n" +
                "- Water = RMB (Orange Fires)\n" +
                "- Put out fires before time runs out!\n\n" +
                "Press Start or Space to Begin";
        }

        StartCoroutine(RandomlyIgnite());
    }

    void Update()
    {
        
        if (!gameStarted && Input.GetKeyDown(KeyCode.Space))
            StartGame();

        if (!gameStarted || gameEnded) return;

        timeRemaining -= Time.unscaledDeltaTime; 
        timerText.text = "Time: " + Mathf.CeilToInt(timeRemaining);

        if (timeRemaining <= 0)
            EndGame();
    }

    public void StartGame()
    {
        gameStarted = true;
        introPanel.SetActive(false);
        Time.timeScale = 1f; 
    }

    IEnumerator RandomlyIgnite()
    {
        while (!gameEnded)
        {
            yield return new WaitForSecondsRealtime(Random.Range(2f, 5f));
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

    public void WrongChoice()
    {
        // Flash red screen / play sound later
        Debug.Log("Wrong extinguisher used!");
    }

    public void EndGame()
    {
        gameEnded = true;
        endPanel.SetActive(true);
        endText.text = "Final Score: " + score;
        Time.timeScale = 0f;
    }
}
