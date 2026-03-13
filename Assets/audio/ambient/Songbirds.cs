using System.Collections.Generic;
using UnityEngine;

public class Songbirds : MonoBehaviour
{
    public Vector2 sunAngleRange;
    public AudioSource forestBirds;

    private void Awake()
    {
        TimeManager.timePassed.AddListener(TimePassed);
    }

    void TimePassed(float time)
    {
        if(DayNightCycle.runtime.sun.transform.eulerAngles.x > sunAngleRange.x &&
            DayNightCycle.runtime.sun.transform.eulerAngles.x < sunAngleRange.y)
        {
            if(!forestBirds.isPlaying) forestBirds.Play();
        }
        else
        {
            forestBirds.Stop();
        }
    }
}
