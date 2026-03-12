using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TemperatureManager : MonoBehaviour
{
    public static TemperatureManager runtime;

    List<TemperatureZone> zones = new List<TemperatureZone>();

    public float sunAngleMult;

    public float winterBaseTemp;
    public float summerBaseTemp;
    public float equinoxBaseTemp;

    public float sunAngleHourOffset = 3f;

    public float dailyRandomness;

    public float ambientTemperature { get; private set; }

    public TextMeshProUGUI txtTemperature;

    int day;
    float variance;

    private void Awake()
    {
        runtime = this;

        GameTick.tick.AddListener(Tick);
    }

    private void Tick()
    {
        //New Day
        if(day != TimeManager.runtime.currentDateTime.Day)
        {
            day = TimeManager.runtime.currentDateTime.Day;
            variance = Random.Range(-dailyRandomness, dailyRandomness);
        }

        float baseTemp;
        if (TimeManager.currentSeason == 4) baseTemp = winterBaseTemp;
        else if (TimeManager.currentSeason == 2) baseTemp = summerBaseTemp;
        else baseTemp = equinoxBaseTemp;

        float t = TimeManager.runtime.currentDateTime.Hour +
                    TimeManager.runtime.currentDateTime.Minute / 60f +
                    TimeManager.runtime.currentDateTime.Second / (60f * 60f);

        float angle = DayNightCycle.runtime.GetSunXRotation(t + sunAngleHourOffset);

        ambientTemperature = angle * sunAngleMult + (baseTemp + variance);

        txtTemperature.text = $"{(int)FindApparentTemp()}°";
    }

    public float FindApparentTemp()
    {
        float apparentTemp = ambientTemperature;
        foreach (TemperatureZone zone in zones)
        {
            if (zone) apparentTemp += zone.GetModifier();
        }
        return apparentTemp;
    }

    public void AddZone(TemperatureZone zone)
    {
        zones.Add(zone);
    }
    public void RemoveZone(TemperatureZone zone)
    {
        zones.Remove(zone);
    }
}
