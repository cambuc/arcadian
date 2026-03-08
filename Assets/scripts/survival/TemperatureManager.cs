using TMPro;
using UnityEngine;

public class TemperatureManager : MonoBehaviour
{
    public static TemperatureManager runtime;

    public int baseTemperature;
    public int ambientTemperature { get; private set; }
    public TextMeshProUGUI txtTemperature;

    private void Awake()
    {
        runtime = this;
    }

    private void Start()
    {
        ambientTemperature = baseTemperature;
    }

    private void Update()
    {
        txtTemperature.text = $"Amb: {ambientTemperature}°\n" +
                              $"Comfort: {PlayerApparel.runtime.idealTemperatureRange.x}° - {PlayerApparel.runtime.idealTemperatureRange.y}°";
    }
}
