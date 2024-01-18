using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private CartMovement _cartMovement;
    private float startTime;
    
    private bool isRunning = false;
    private bool onetime = true;
    private bool endtimer = false;

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

    string FormatTime(float timeToFormat)
    {
        int minutes = (int)timeToFormat / 60;
        int seconds = (int)timeToFormat % 60;
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
}