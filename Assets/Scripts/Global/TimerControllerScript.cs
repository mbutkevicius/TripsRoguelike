using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerControllerScript : MonoBehaviour
{
    public static TimerControllerScript instance;
    public TextMeshProUGUI timerText;

    private TimeSpan timePlaying;

    private bool timerGoing;

    private float elapsedTime;
    private float ghostElapsedTime;

    // Use this for multiplying the ghosts' speeds over time
    public float ghostTimeFraction;

    [SerializeField] private float maxGhostTime;
    [SerializeField] private float ghostSpeedMultiplier;

    private void Update()
    {
        // Calculates the multiplier for ghost speed
        if (elapsedTime <= maxGhostTime)
        {
            ghostElapsedTime = elapsedTime;
            ghostTimeFraction = (ghostElapsedTime / maxGhostTime * (ghostSpeedMultiplier - 1)) + 1;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
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
