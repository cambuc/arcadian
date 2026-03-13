using System.Collections.Generic;
using UnityEngine;

public class CookingProduct : Interactable
{
    public Campfire campfire;
    public CookingRecipe recipe;
    public float timeToCook;

    public Renderer productRenderer;
    public Material unfinishedMat;
    public Material finishedMat;

    private void Awake()
    {
        TimeManager.timePassed.AddListener(TimePassed);
    }

    public override void SetInteractOptions()
    {
        timeToCook = recipe.timeInHours * 3600f;
        interactName = recipe.product.itemName;

        productRenderer.material = unfinishedMat;

        interactOptions.Clear();
        interactOptions.Add(new InteractOption()
        {
            text = "Pick Up"
        });
    }

    public override void Interact(string interactOption)
    {
        SoundPlayerHolder.runtime.fleshClick.PlaySound();
        if(timeToCook > 0)
        {
            foreach(ItemQuantity iq in recipe.components)
                for (int i = 0; i < iq.amount; i++)
                    PlayerInventory.runtime.AddItem(iq.item);
        }
        else
        {
            PlayerInventory.runtime.AddItem(recipe.product);
        }
        Destroy(gameObject);
    }

    void TimePassed(float seconds)
    {
        if (campfire.fireTime <= 0) return;

        timeToCook -= seconds;

        if(timeToCook <= 0)
        {
            extraText = recipe.drying ? "Fully Dried" : "Fully Cooked";
            productRenderer.material = finishedMat;
            return;
        }

        float minutes = timeToCook / 60f;

        string hours = "";
        if (minutes >= 60) hours = $"{(int)(minutes / 60f)} hr ";
        extraText = hours + $"{(int)(minutes % 60f)} min until {(recipe.drying ? "dried" : "cooked")}";
    }
}
