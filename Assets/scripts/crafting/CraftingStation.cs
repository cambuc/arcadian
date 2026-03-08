using System.Collections.Generic;
using UnityEngine;

public class CraftingStation : Interactable
{
    public List<CraftingRecipe> recipes = new List<CraftingRecipe>();

    public override void Interact(string interactOption)
    {
            CraftingMenu.runtime.OpenMenu(recipes, this);
    }
}
