using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SurvivalAttributes : MonoBehaviour
{
    public static SurvivalAttributes runtime;

    public float temperature { get; private set; }
    public float thirst { get; private set; }
    public float hunger { get; private set; }
    public float fatigue { get; private set; }

    [Header("UI Elements")]
    public ValueBar tempBar;
    public ValueBar thirstBar;
    public ValueBar hungerBar;
    public ValueBar fatigueBar;
    //public TextMeshProUGUI txtTempGain;
    //public TextMeshProUGUI txtThirstGain;
    //public TextMeshProUGUI txtHungerGain;
    //public TextMeshProUGUI txtFatigueGain;

    [Header("Gain Rates (Hours Until Maxed Without Activity)")]
    public Vector2 charCountThresholds;
    public int drainCallLoopSeconds;
    public float multDrainTemperature;
    public float baseGainRateThirst;
    public float baseGainRateHunger;
    public float baseGainRateFatigue;

    public Dictionary<string, float> addersTemperature = new Dictionary<string, float>();
    public Dictionary<string, float> multipliersThirst = new Dictionary<string, float>();
    public Dictionary<string, float> multipliersHunger = new Dictionary<string, float>();
    public Dictionary<string, float> multipliersFatigue = new Dictionary<string, float>();

    private void Awake()
    {
        runtime = this;
    }

    private void Start()
    {
        UpdateUI();
        TimeManager.runtime.WaitToCallLoop(new WaitCallLoop() { action = Drain, loopDuration = drainCallLoopSeconds });
    }

    void Drain()
    {
        //Debug.Log($"{TimeManager.runtime.currentDateTime.ToLongTimeString()} | {thirst}");
        AddTemperature(GetDrainTemperature());
        AddThirst((1 / (baseGainRateThirst * 720)) * GetMultThirst());
        AddHunger((1 / (baseGainRateHunger * 720)) * GetMultHunger());
        AddFatigue((1 / (baseGainRateFatigue * 720)) * GetMultFatigue());

        if (thirst >= 1) DeathScreen.runtime.ShowDeathScreen("You succumbed to dehydration");
        if (hunger >= 1) DeathScreen.runtime.ShowDeathScreen("You succumbed to starvation");
    }

    private void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        tempBar.Value = (temperature + 1) / 2f;
        thirstBar.Value = thirst;
        hungerBar.Value = hunger;
        fatigueBar.Value = fatigue;

        //txtTempGain.text = GetGainText(GetDrainTemperature());
        //txtThirstGain.text = GetGainText(GetMultThirst());
        //txtHungerGain.text = GetGainText(GetMultHunger());
        //txtFatigueGain.text = GetGainText(GetMultFatigue());
    }

    public void AddTemperature(float amount)
    {
        temperature = Mathf.Clamp(temperature + amount, -1, 1);
    }
    public void AddThirst(float amount)
    {
        thirst = Mathf.Clamp01(thirst + amount);
    }
    public void AddHunger(float amount)
    {
        hunger = Mathf.Clamp01(hunger + amount);
    }
    public void AddFatigue(float amount)
    {
        fatigue = Mathf.Clamp01(fatigue + amount);
    }

    public float GetDrainTemperature()
    {
        int amb = TemperatureManager.runtime.ambientTemperature;
        int low = PlayerApparel.runtime.idealTemperatureRange.x;
        int high = PlayerApparel.runtime.idealTemperatureRange.y;

        float adder = (amb > high ? amb - high : amb < low ? amb - low : 0) * multDrainTemperature;

        foreach (float add in addersTemperature.Values)
        {
            adder += add;
        }
        return adder;
    }
    public float GetMultThirst()
    {
        float gainMult = 1;
        foreach (float mult in multipliersThirst.Values)
        {
            gainMult += mult;
        }
        return gainMult;
    }
    public float GetMultHunger()
    {
        float gainMult = 1;
        foreach (float mult in multipliersHunger.Values)
        {
            gainMult += mult;
        }
        return gainMult;
    }
    public float GetMultFatigue()
    {
        float gainMult = 1;
        foreach (float mult in multipliersFatigue.Values)
        {
            gainMult += mult;
        }
        return gainMult;
    }

    public string GetGainText(float value)
    {
        if (value == 0) return "";
        else if(value < 0)
        {
            if(value <= -charCountThresholds.x)
            {
                if (value <= -charCountThresholds.y) { return "---"; }
                else { return "--"; }
            }
            else { return "-"; }
        }
        else
        {
            if (value >= charCountThresholds.x)
            {
                if (value >= charCountThresholds.y) { return "+++"; }
                else { return "++"; }
            }
            else { return "+"; }
        }
    }
}
