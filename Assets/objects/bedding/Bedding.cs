using System.Collections.Generic;
using UnityEngine;

public class Bedding : Interactable
{
    public Vector2 counterRange;
    public float counterStep = 0.5f;
    public float hourFatigueMult = 0.1f;
    public float survivalDrainMult = -0.5f;

    public override void Interact(string interactOption)
    {
        interactable = false;
        CounterWindow.runtime.OpenWindow(counterRange.x, counterRange.y, counterStep, counterRange.x, AmountSelected);
    }

    public void AmountSelected(float amount)
    {
        interactable = true;

        if (amount < 0) return;

        Fader.runtime.FadeOut(() =>
        {
            SurvivalAttributes.runtime.multipliersThirst.Add("resting", survivalDrainMult);
            SurvivalAttributes.runtime.multipliersHunger.Add("resting", survivalDrainMult);
            SurvivalAttributes.runtime.multipliersFatigue.Add("resting", survivalDrainMult);

            TimeManager.runtime.PassTime(60 * 60 * amount);
            SurvivalAttributes.runtime.AddFatigue(-hourFatigueMult * amount);
            Fader.runtime.FadeIn();

            SurvivalAttributes.runtime.multipliersThirst.Remove("resting");
            SurvivalAttributes.runtime.multipliersHunger.Remove("resting");
            SurvivalAttributes.runtime.multipliersFatigue.Remove("resting");
        });
    }
}
