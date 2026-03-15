using System.Collections.Generic;
using UnityEngine;

public class CraftingStation : Interactable
{
    public List<CraftingRecipe> recipes = new List<CraftingRecipe>();

    public override void SetInteractOptions()
    {
        interactOptions.Add(new InteractOption() { text = "Tear Down", interactTime = 0.5f });
    }

    public override void Interact(string interactOption)
    {
        if (interactOption == "Tear Down")
        {
            Destroy(gameObject);
            return;
        }

        CraftingMenu.runtime.OpenMenu(recipes, this);
    }
}
