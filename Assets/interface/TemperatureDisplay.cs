using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TemperatureDisplay : MonoBehaviour
{
    public RawImage vignette;

    public Color hotColor;
    public Color coldColor;

    private void Awake()
    {
        GameTick.tick.AddListener(UpdateDisplay);
    }

    void UpdateDisplay()
    {
        float temp = SurvivalAttributes.runtime.temperature;
        vignette.color = new Color(
            temp > 0 ? hotColor.r : coldColor.r,
            temp > 0 ? hotColor.g : coldColor.g,
            temp > 0 ? hotColor.b : coldColor.b,
            Mathf.Abs(temp)
            );
    }
}
