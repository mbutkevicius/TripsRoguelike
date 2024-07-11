using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameDataManager : MonoBehaviour
{
    [Header("Score Data")]
    public Text scoreText;
    public Text highScoreText;
    public int score;

    [Header("Timer Data")]
    [Tooltip("Amount of time it takes for ghosts to reach max speed")]
    [SerializeField] private float ghostMaxSpeedTime;
    [Tooltip("Number that ghost speed will be multiplied by when ghostMaxSpeedTime is reached (starts increasing from start of timer)")]
    [SerializeField] private float ghostSpeedMultiplier;
    public static GameDataManager instance;
    private TimeSpan timePlaying;
    private bool timerGoing;
    private float elapsedTime;
    private float ghostElapsedTime;
    [Tooltip("Public variable to be used as a reference in other ghost scripts. Multiply their movement by this")]
    public float ghostTimeFraction;

    [Header("Font References")]
    public Text timerText;

    [HideInInspector] public float speedTimeMultiplier;

    private void Update()
    {
        // Calculates the multiplier for ghost speed
        if (elapsedTime <= ghostMaxSpeedTime)
        {
            ghostElapsedTime = elapsedTime;
            ghostTimeFraction = (ghostElapsedTime / ghostMaxSpeedTime * (ghostSpeedMultiplier - 1)) + 1;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        //timerText = GameObject.FindGameObjectWithTag("HUD").GetComponent<TextMeshProUGUI>();

        timerText.text = "00:00.00";
        timerGoing = false;

        UnityEngine.SceneManagement.Scene currentScene = SceneManager.GetActiveScene();

        // Check the name of the current scene
        if (currentScene.name == "TitleScreen")
        {
            highScoreText.text = PlayerPrefs.GetInt("HighScore", 0).ToString();
        }
    }

    public void BeginTimer()
    {
        timerGoing = true;
        elapsedTime = 0f;

        StartCoroutine(UpdateTimer());
    }

    public void EndTimer()
    {
        timerGoing = false;
    }

    private IEnumerator UpdateTimer()
    {
        while (timerGoing)
        {
            elapsedTime += Time.deltaTime;
            timePlaying = TimeSpan.FromSeconds(elapsedTime);
            string timePlayingStr = timePlaying.ToString("mm':'ss'.'ff");
            timerText.text = timePlayingStr;

            yield return null;
        }
    }

    public void SetHighScore()
    {
        if (score > PlayerPrefs.GetInt("HighScore", 0))
        PlayerPrefs.SetInt("HighScore", score);
    }

    [ContextMenu("ResetHighScore")]
    public void ResetHighScore()
    {
        PlayerPrefs.SetInt("HighScore", 0);
    }
}
