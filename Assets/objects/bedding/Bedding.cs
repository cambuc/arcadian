using System.Collections.Generic;
using UnityEngine;

public class Bedding : Interactable
{
    public Vector2 counterRange;
    public float counterStep = 0.5f;
    public float hourFatigueMult = 0.1f;
    public float warmthBonus;
    public float survivalDrainMult = -0.5f;

    public override void SetInteractOptions()
    {
        interactOptions.Clear();
        interactOptions.Add(new InteractOption()
        {
            text = "Rest"
        });
        interactOptions.Add(new InteractOption()
        {
            text = "Tear Down",
            interactTime = 0.5f
        });
    }

    public override void Interact(string interactOption)
    {
        if(interactOption == "Rest")
        {
            interactable = false;
            CounterWindow.runtime.OpenWindow(counterRange.x, counterRange.y, counterStep, counterRange.x, AmountSelected);
        }
        else if (interactOption == "Tear Down")
        {
            Destroy(gameObject);
        }
    }

    public void AmountSelected(float amount)
    {
        interactable = true;

        if (amount < 0) return;

        Fader.runtime.FadeOut(() =>
        {
            SurvivalAttributes.runtime.addersTemperature.Add("resting", warmthBonus);
            SurvivalAttributes.runtime.multipliersThirst.Add("resting", survivalDrainMult);
            SurvivalAttributes.runtime.multipliersHunger.Add("resting", survivalDrainMult);
            SurvivalAttributes.runtime.multipliersFatigue.Add("resting", survivalDrainMult);

            for(float i = 0; i < amount; i += counterStep)
            {
                TimeManager.runtime.PassTime(3600f * counterStep);
                SurvivalAttributes.runtime.AddFatigue(-hourFatigueMult * amount);
                Fader.runtime.FadeIn();
            }

            SurvivalAttributes.runtime.addersTemperature.Remove("resting");
            SurvivalAttributes.runtime.multipliersThirst.Remove("resting");
            SurvivalAttributes.runtime.multipliersHunger.Remove("resting");
            SurvivalAttributes.runtime.multipliersFatigue.Remove("resting");
        });
    }
}
