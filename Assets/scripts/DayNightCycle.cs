using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public Light sun;

    public float maxSunXRotation;
    public float maxSunYRotation;

    public float sunBaseIntensity;
    public float sunIntensityAmplitude;

    public Color sunLowColor;
    public Color sunHighColor;

    public Color fogDayColor;
    public Color fogDawnColor;
    public Color fogNightColor;
    public Vector2 dayTimeRange;
    public float dayTimeColorWeight = 3;

    private void Awake()
    {
        GameTick.tick.AddListener(AdjustPositions);
    }

    void AdjustPositions()
    {
        float t =   TimeManager.runtime.currentDateTime.Hour + 
                    TimeManager.runtime.currentDateTime.Minute / 60f +
                    TimeManager.runtime.currentDateTime.Second / (60f * 60f);

        sun.transform.eulerAngles = new Vector3(GetSunXRotation(t), GetSunYRotation(t), 0);
        sun.intensity = GetSunIntensity(t);
        sun.color = GetSunColor(t);

        RenderSettings.fogColor = GetFogColor(t);
    }

    float GetSunXRotation(float hour)
    {
        return -Mathf.Cos((Mathf.PI * hour) / 12) * maxSunXRotation;
    }
    float GetSunYRotation(float hour)
    {
        return -Mathf.Sin((Mathf.PI * hour) / 12) * maxSunYRotation;
    }
    float GetSunIntensity(float hour)
    {
        return sunIntensityAmplitude * - Mathf.Cos((Mathf.PI * hour) / 12) + sunBaseIntensity;
    }
    Color GetSunColor(float hour)
    {
        return Color.Lerp(sunLowColor, sunHighColor, -Mathf.Cos((Mathf.PI * hour) / 12));
    }

    Color GetFogColor(float hour)
    {
        if(hour >= dayTimeRange.x && hour < dayTimeRange.y)
        {
            float range = dayTimeRange.y - dayTimeRange.x;
            float midday = dayTimeRange.x + (range / 2f);

            float t = Mathf.Abs(hour - midday) / (range / 2f);
            t = Mathf.Pow(t, dayTimeColorWeight);

            return Color.Lerp(fogDayColor, fogDawnColor, t);
        }
        else
        {
            float range = 24 - (dayTimeRange.y - dayTimeRange.x);
            float midnight = (dayTimeRange.y + (range / 2f));
            float h = hour < dayTimeRange.x ? hour + 24 : hour;

            float t = Mathf.Abs(h - midnight) / (range / 2f);
            t = Mathf.Pow(t, dayTimeColorWeight);

            return Color.Lerp(fogNightColor, fogDawnColor, t);
        }
    }
}
