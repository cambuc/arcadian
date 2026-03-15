using System.Collections.Generic;
using System.ComponentModel;
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
    public float showIndicatorThreshold;
    public GameObject tempIndicator;
    public GameObject thirstIndicator;
    public GameObject hungerIndicator;
    public GameObject fatigueIndicator;
    public ValueBar tempBar;
    public ValueBar thirstBar;
    public ValueBar hungerBar;
    public ValueBar fatigueBar;
    //public TextMeshProUGUI txtTempGain;
    //public TextMeshProUGUI txtThirstGain;
    //public TextMeshProUGUI txtHungerGain;
    //public TextMeshProUGUI txtFatigueGain;

    [Header("Gain Rates")]
    public Vector2 charCountThresholds;
    public float multDrainTemperature;
    public float multRecoverTemperature;
    public float thirstHoursToMax;
    public float hungerHoursToMax;
    public float fatigueHoursToMax;

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
        TimeManager.timePassed.AddListener(Drain);
    }

    void Drain(float timePassed)
    {
        //Debug.Log($"{TimeManager.runtime.currentDateTime.ToLongTimeString()} | {thirst}");
        AddTemperature(GetDrainTemperature() * timePassed);
        AddThirst((timePassed / (thirstHoursToMax * 3600)) * GetMultThirst());
        AddHunger((timePassed / (hungerHoursToMax * 3600)) * GetMultHunger());
        AddFatigue((timePassed / (fatigueHoursToMax * 3600)) * GetMultFatigue());

        if (thirst >= 1 && DeathScreen.runtime) DeathScreen.runtime.ShowDeathScreen("You succumbed to dehydration");
        if (hunger >= 1 && DeathScreen.runtime) DeathScreen.runtime.ShowDeathScreen("You succumbed to starvation");
    }

    private void Update()
    {
        UpdateUI();

        if (PlayerHud.runtime.wheelRoot.activeSelf)
        {
            tempIndicator.SetActive(true);
            thirstIndicator.SetActive(true);
            hungerIndicator.SetActive(true);
            fatigueIndicator.SetActive(true);
        }
        else
        {
            tempIndicator.SetActive(Mathf.Abs(temperature) >= showIndicatorThreshold);
            thirstIndicator.SetActive(thirst >= showIndicatorThreshold);
            hungerIndicator.SetActive(hunger >= showIndicatorThreshold);
            fatigueIndicator.SetActive(fatigue >= showIndicatorThreshold);
        }
    }

    private void UpdateUI()
    {
        tempBar.Value = 1 - Mathf.Abs(temperature);
        thirstBar.Value = 1 - thirst;
        hungerBar.Value = 1 - hunger;
        fatigueBar.Value = 1 - fatigue;

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

    float tempConstant = 1f / 100000f;
    public float GetDrainTemperature()
    {
        float amb = TemperatureManager.runtime.FindApparentTemp();
        float low = PlayerApparel.runtime.idealTemperatureRange.x;
        float high = PlayerApparel.runtime.idealTemperatureRange.y;
        float ideal = (low + high) / 2f;

        foreach (float add in addersTemperature.Values)
        {
            amb += add;
        }
        float adder = (amb > high ? amb - high : amb < low ? amb - low : 0) * (multDrainTemperature * tempConstant);
        if (adder == 0) adder = -Ceiling(temperature) * (multRecoverTemperature * tempConstant);
        if ((temperature > 0 && amb < low) || temperature < 0 && amb > high)
            adder *= multRecoverTemperature;

        return adder;
    }
    int Ceiling(float num)
    {
        if(num < 0)
        {
            return -Mathf.CeilToInt(-num);
        }
        return Mathf.CeilToInt(num);
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
