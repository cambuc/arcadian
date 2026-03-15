using System.Collections.Generic;
using UnityEngine;

public class UnfinishedProduct : Interactable
{
    CraftingRecipe recipe;
    WorldItem worldItem;

    int totalClicks;
    int clicks;

    float timeStepHrs;

    public void Initialize(CraftingRecipe recipe, WorldItem worldItem)
    {
        this.recipe = recipe;
        this.worldItem = worldItem;
        worldItem.interactable = false;

        totalClicks = Mathf.Max(1, (int)(recipe.timeInHours * 12f));
        clicks = 0;
        timeStepHrs = 1f / 12f;

        if (GetProgress() >= 100 || totalClicks == 0)
        {
            Complete();
            return;
        }

        interactName = $"{recipe.product.itemName}";
        extraText = $"{GetProgress()}% Complete";

        gameObject.layer = 10;
        foreach(Transform child in GetComponentsInChildren<Transform>())
        {
            child.gameObject.layer = 10;
        }

        SetInteractOptions();
    }

    public override void SetInteractOptions()
    {
        interactOptions.Clear();
        interactOptions.Add(new InteractOption()
        {
            text = $"Craft ({(int)(timeStepHrs * 60f)} min)"
        });
        interactOptions.Add(new InteractOption()
        {
            text = "Grab"
        });
    }

    public override void Interact(string interactOption)
    {
        

        if (interactOption == "Grab")
        {
            Grab();
            SoundPlayerHolder.runtime.sfxSelect.PlaySound();
            return;
        }

        SoundPlayerHolder.runtime.sfxClick.PlaySound();
        clicks++;
        TimeManager.runtime.PassTime(timeStepHrs * 3600f);

        if(GetProgress() >= 100)
        {
            gameObject.layer = 0;
            foreach (Transform child in GetComponentsInChildren<Transform>())
            {
                child.gameObject.layer = 0;
            }

            SoundPlayerHolder.runtime.woodCarve.PlaySound();
            Complete();
            return;
        }

        extraText = $"{GetProgress()}% Complete";
    }

    void Complete()
    {
        interactable = false;
        worldItem.interactable = true;
    }

    int GetProgress()
    {
        return (int)((float)clicks / totalClicks * 100f);
    }
}
