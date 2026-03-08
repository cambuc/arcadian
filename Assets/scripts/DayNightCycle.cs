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

    private void Awake()
    {
        GameTick.tick.AddListener(AdjustPositions);
    }

    void AdjustPositions()
    {
        float t = TimeManager.runtime.currentDateTime.Hour + 
            TimeManager.runtime.currentDateTime.Minute / 60f +
             +TimeManager.runtime.currentDateTime.Second / (60f * 60f);
        sun.transform.eulerAngles = new Vector3(GetSunXRotation(t), GetSunYRotation(t), 0);
        sun.intensity = GetSunIntensity(t);
        sun.color = GetSunColor(t);
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
}
