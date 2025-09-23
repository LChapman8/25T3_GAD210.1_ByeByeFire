using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// This script manages my entire game loop, it takes in all of my assets from the scene and scripts to manage the flow of the game
/// </summary>

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // all my references to UI pieces 
    [Header("UI")]
    public TMP_Text timerText;
    public TMP_Text scoreText;
    public TMP_Text mistakesText; 
    public GameObject endPanel;
    public TMP_Text endText;
    public TMP_Text endMistakesText; 
    public Button replayButton;

    [Header("Intro UI")]
    public GameObject introPanel;
    public Button startButton;

    // array where my cube (the fires) are added
    [Header("Fires")]
    public Fire[] fires;
    // variable for the game length
    [Header("Settings")]
    public float gameTime = 30f;
    // player feedback red screen flash if they get one wrong
    [Header("Effects")]
    public Image redFlashOverlay;
    // variables for tracking scores and mistakes and for the game start/end
    private int score = 0;
    private int mistakes = 0; 
    private float timeRemaining;
    public bool gameEnded = false;
    public bool gameStarted = false;

    private Coroutine fireRoutine;

    void Awake()
    {
        Instance = this;
    }
    // on start set game states
    void Start()
    {
        timeRemaining = gameTime;
        scoreText.text = "Score: 0";
        mistakesText.text = "Mistakes: 0"; 
        endPanel.SetActive(false);
        // pause the game until start is pressed
        if (introPanel != null)
        {
            introPanel.SetActive(true);
            Time.timeScale = 0f;
        }
        // adds listner to start
        if (startButton != null)
            startButton.onClick.AddListener(StartGame);
        //adds listenr to end
        if (replayButton != null)
            replayButton.onClick.AddListener(ReplayGame);
        // flash red screen on failed attempt
        if (redFlashOverlay != null)
            redFlashOverlay.color = new Color(1, 0, 0, 0);
    }

    void Update()
    {
        //press space to start
        if (!gameStarted && Input.GetKeyDown(KeyCode.Space))
            StartGame();

        // press space to restart game 
        if (gameEnded && Input.GetKeyDown(KeyCode.Space))
            ReplayGame();

        if (!gameStarted || gameEnded) return;

        timeRemaining -= Time.deltaTime;
        timerText.text = "Time: " + Mathf.CeilToInt(timeRemaining);

        if (timeRemaining <= 0)
            EndGame();
    }

    // on start set all game stats, start the fire coroutine 
    public void StartGame()
    {
        gameStarted = true;
        introPanel.SetActive(false);
        endPanel.SetActive(false);
        Time.timeScale = 1f;

        if (fireRoutine == null)
            fireRoutine = StartCoroutine(RandomlyIgnite());
    }

    // randomly ignite cubes on fire 
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
    // score manager for success 
    public void AddScore(int amount)
    {
        score += amount;
        scoreText.text = "Score: " + score;
    }
    //score manager for failures
    public void WrongChoice(Fire fire)
    {
       
        mistakes++;
        if (mistakesText != null)
            mistakesText.text = "Mistakes: " + mistakes;

      
        fire.SetFire(Fire.FireType.None);

       
        if (redFlashOverlay != null)
            StartCoroutine(FlashRed());
    }
    // ienumerator for failed shot, makes screen flash red briefly 
    IEnumerator FlashRed()
    {
        redFlashOverlay.color = new Color(1, 0, 0, 0.5f);
        yield return new WaitForSeconds(0.25f);
        redFlashOverlay.color = new Color(1, 0, 0, 0f);
    }
    // function for ending the game at end of timer
    public void EndGame()
    {
        gameEnded = true;
        endPanel.SetActive(true);
        endText.text = "Final Score: " + score;
        if (endMistakesText != null)
            endMistakesText.text = "Mistakes: " + mistakes;
        Time.timeScale = 0f;
    }
    // function to restart the game 
    public void ReplayGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
