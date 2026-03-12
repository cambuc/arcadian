using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public static DayNightCycle runtime;

    public Light sun;

    public float maxSunYRotation;
    public float maxSunXRotation = 50f;

    public float sunBaseIntensity;

    public Color sunLowColor;
    public Color sunHighColor;

    [Header("Seasonal")]
    public float summerSunIntensity;
    public float winterSunIntensity;

    [Header("Fog")]
    public Color fogDayColor;
    public Color fogDawnColor;
    public Color fogNightColor;
    public float angleThreshold;

    private void Awake()
    {
        runtime = this;

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

        RenderSettings.fogColor = GetFogColor(sun.transform.eulerAngles.x);
    }

    public float GetSunXRotation(float hour)
    {
        float offset;
        if (TimeManager.currentSeason == 4) offset = -23.5f;
        else if (TimeManager.currentSeason == 2) offset = 23.5f;
        else offset = 0;

        return (-Mathf.Cos((Mathf.PI * hour) / 12) * maxSunXRotation) + offset;
    }
    float GetSunYRotation(float hour)
    {
        return -Mathf.Sin((Mathf.PI * hour) / 12) * maxSunYRotation;
    }
    float GetSunIntensity(float hour)
    {
        float amp;
        if (TimeManager.currentSeason == 4) amp = winterSunIntensity;
        else if (TimeManager.currentSeason == 2) amp = summerSunIntensity;
        else amp = (summerSunIntensity + winterSunIntensity) / 2f;

        return amp * - Mathf.Cos((Mathf.PI * hour) / 12) + sunBaseIntensity;
    }
    Color GetSunColor(float hour)
    {
        return Color.Lerp(sunLowColor, sunHighColor, -Mathf.Cos((Mathf.PI * hour) / 12));
    }

    Color GetFogColor(float angle)
    {
        while (angle >= 180) angle -= 360;

        if(angle >= 0)
        {
            return Color.Lerp(fogDawnColor, fogDayColor, angle / angleThreshold);
        }
        else
        {
            return Color.Lerp(fogDawnColor, fogNightColor, Mathf.Abs(angle / angleThreshold));
        }
    }
}
