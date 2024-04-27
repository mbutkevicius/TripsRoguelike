using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameDataManager : MonoBehaviour
{
    [Header("Score Data")]
    public TextMeshProUGUI scoreText;
    [HideInInspector] public int score;

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
    public TextMeshProUGUI timerText;

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

        timerText.text = "Time - 00:00.00";
        timerGoing = false;
        BeginTimer();
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
            string timePlayingStr = "Time: " + timePlaying.ToString("mm':'ss'.'ff");
            timerText.text = timePlayingStr;

            yield return null;
        }
    }
}
