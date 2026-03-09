using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour
{
    public static TimeManager runtime;

    public DateTimeSerialized startDateTime;
    public DateTime currentDateTime { get; private set; }

    public float timeMult;

    public List<WaitCall> waitCalls = new List<WaitCall>();
    public List<WaitCallLoop> waitCallLoops = new List<WaitCallLoop>();

    public bool paused { get; private set; }

    private void Awake()
    {
        runtime = this;

        GameTick.tick.AddListener(Tick);

        currentDateTime = new DateTime(startDateTime.year, startDateTime.month, startDateTime.day, startDateTime.hour, startDateTime.minute, 0);
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

        for (int i = 0; i < waitCallLoops.Count; i++)
        {
            waitCallLoops[i] = new WaitCallLoop() { action = waitCallLoops[i].action, secondsLeft = waitCallLoops[i].secondsLeft - timeInSeconds, loopDuration = waitCallLoops[i].loopDuration };

            //int invocations = timeInSeconds >= waitCallLoops[i].secondsLeft ? 1 : 0;
            int invocations = (int)((timeInSeconds - waitCallLoops[i].secondsLeft) / waitCallLoops[i].loopDuration);

            if(invocations > 0)
                waitCallLoops[i] = new WaitCallLoop() { action = waitCallLoops[i].action, secondsLeft = waitCallLoops[i].loopDuration, loopDuration = waitCallLoops[i].loopDuration };

            for (int n = 0; n < invocations; n++)
                waitCallLoops[i].action.Invoke();
        }
    }

    public void WaitToCall(WaitCall waitCall)
    {
        waitCalls.Add(waitCall);
    }
    public void WaitToCallLoop(WaitCallLoop waitCall)
    {
        if(waitCall.secondsLeft <= 0)
            waitCall.secondsLeft = waitCall.loopDuration;
        waitCallLoops.Add(waitCall);
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
public struct WaitCallLoop
{
    public UnityAction action;
    public float secondsLeft;
    public float loopDuration;
}
