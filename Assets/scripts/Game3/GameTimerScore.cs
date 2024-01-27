using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameTimerScore : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private CartMovement _cartMovement;
    [SerializeField] private TextMeshProUGUI CorrectText;
    [SerializeField] private TextMeshProUGUI scoreText;
    private float startTime;
    
    private bool isRunning = false;
    private bool onetime = true;
    private bool endtimer = false;
    private int score;
    private int minutes;
    private int seconds;

    private void Start()
    {
        score = 0;
        minutes = 0;
        seconds = 0;
    }

    void Update()
    {
        if (endtimer)
        {
            StopStopwatch();
        }
        else
        {
            if (_cartMovement.Target && onetime)
            {
                onetime = false;
                StartStopwatch();
            }

            if (isRunning)
            {
                float timeElapsed = Time.time - startTime;
                string formattedTime = FormatTime(timeElapsed);
                timerText.text = formattedTime;
            }
        }
    }


    public void ScoreCounter()
    {
        score++;
        CorrectText.text = score.ToString();
    }

    string FormatTime(float timeToFormat)
    {
        minutes = (int)timeToFormat / 60;
        seconds = (int)timeToFormat % 60;
        string formattedTime = string.Format("{0:00}:{1:00}", minutes, seconds);
        return formattedTime;
    }

    public void StartStopwatch()
    {
        if (_cartMovement.Target)
        {
            isRunning = true;
            startTime = Time.time;
        }
    }

    public void StopStopwatch()
    {
        isRunning = false;
    }

    public void FinalScore()
    {
        scoreText.text = (score * 1000 / seconds).ToString();
    }
}
