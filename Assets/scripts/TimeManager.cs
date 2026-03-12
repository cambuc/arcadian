using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour
{
    public static TimeManager runtime;
    public static UnityEvent<float> timePassed = new UnityEvent<float>();

    public static UnityEvent seasonChanged = new UnityEvent();
    public static int currentSeason;

    public float daysPerSeason = 10;

    public bool SET_DATETIME_RUNTIME;
    public DateTimeSerialized startDateTime;
    public DateTime currentDateTime { get; private set; }

    public float timeMult;

    public TextMeshProUGUI labelDate;
    public TextMeshProUGUI labelTime;

    public List<WaitCall> waitCalls = new List<WaitCall>();

    public bool paused { get; private set; }

    private void Awake()
    {
        runtime = this;
        currentSeason = -1;

        GameTick.tick.AddListener(Tick);

        currentDateTime = new DateTime(startDateTime.year, startDateTime.month, startDateTime.day, startDateTime.hour, startDateTime.minute, 0);
        UpdateSeason();
    }

    void UpdateSeason()
    {
        currentSeason = GetSeason();

        if (currentSeason == 1) labelDate.text = "Spring";
        else if (currentSeason == 2) labelDate.text = "Summer";
        else if (currentSeason == 3) labelDate.text = "Fall";
        else labelDate.text = "Winter";

        seasonChanged.Invoke();
    }

    private void OnValidate()
    {
        if (SET_DATETIME_RUNTIME)
        {
            SET_DATETIME_RUNTIME = false;
            currentDateTime = new DateTime(startDateTime.year, startDateTime.month, startDateTime.day, startDateTime.hour, startDateTime.minute, 0);
        }
    }

    public void PauseTime()
    {
        paused = true;
    }
    public void UnpauseTime()
    {
        paused = false;
    }

    void Tick()
    {
        if (paused || timeMult <= 0)
            return;

        UpdateTime();
    }

    void UpdateTime()
    {
        PassTime(timeMult * (GameTick.runtime.tickLength / 1000f));
    }

    public void PassTime(float timeInSeconds)
    {
        currentDateTime = currentDateTime.AddSeconds(timeInSeconds);

        List<WaitCall> calls = new List<WaitCall>();
        for (int i = 0; i < waitCalls.Count; i++)
        {
            waitCalls[i] = new WaitCall() { action = waitCalls[i].action, secondsLeft = waitCalls[i].secondsLeft - timeInSeconds };
            if (waitCalls[i].secondsLeft <= 0)
            {
                calls.Add(waitCalls[i]);
                waitCalls.RemoveAt(i);
                i--;
            }
        }
        foreach (WaitCall call in calls)
            call.action.Invoke();

        labelTime.text = currentDateTime.ToShortTimeString();

        float value = (float)currentDateTime.Month + currentDateTime.Day / 100f + currentDateTime.Hour / 10000f;
        float length = daysPerSeason / 100f;

        if (value < 3.21f + length) value += 12f;

        if (value >= 3.21f + length && value < 6.21f + length && currentSeason != 2)
        {
            currentDateTime = new DateTime(currentDateTime.Year, 6, 21,
                                            currentDateTime.Hour, currentDateTime.Minute, currentDateTime.Second);
            UpdateSeason();
        }
        else if (value >= 6.21f + length && value < 9.21f + length && currentSeason != 3)
        {
            currentDateTime = new DateTime(currentDateTime.Year, 9, 21,
                                            currentDateTime.Hour, currentDateTime.Minute, currentDateTime.Second);
            UpdateSeason();
        }
        else if (value >= 9.21f + length && value < 12.21f + length && currentSeason != 4)
        {
            currentDateTime = new DateTime(currentDateTime.Year, 12, 21,
                                            currentDateTime.Hour, currentDateTime.Minute, currentDateTime.Second);
            UpdateSeason();
        }
        else if (value >= 12.21f + length && currentSeason != 1)
        {
            currentDateTime = new DateTime(currentDateTime.Year + 1, 3, 21,
                                            currentDateTime.Hour, currentDateTime.Minute, currentDateTime.Second);
            UpdateSeason();
        }

        timePassed.Invoke(timeInSeconds);
    }
    private int GetSeason()
    {
        float value = (float)currentDateTime.Month + currentDateTime.Day / 100f + currentDateTime.Hour / 10000f;
        if (value >= 3.21f && value < 6.21f)
            return 1;
        else if (value >= 6.21f && value < 9.21f)
            return 2;
        else if (value >= 9.21f && value < 12.21f)
            return 3;
        else
            return 4;
    }

    public void WaitToCall(WaitCall waitCall)
    {
        waitCalls.Add(waitCall);
    }
}

[System.Serializable]
public struct DateTimeSerialized
{
    public int year;
    [Range(1, 12)]
    public int month;
    [Range(1, 31)]
    public int day;

    [Range(0, 23)]
    public int hour;
    [Range(0, 60)]
    public int minute;
}
public struct WaitCall
{
    public UnityAction action;
    public float secondsLeft;
}
