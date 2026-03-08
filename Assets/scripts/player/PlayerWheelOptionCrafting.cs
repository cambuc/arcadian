using System.Collections.Generic;
using UnityEngine;

public class PlayerWheelOptionCrafting : PlayerWheelOption
{
    public List<CraftingRecipe> recipes = new List<CraftingRecipe>();

    public override void Open()
    {
        CraftingMenu.runtime.OpenMenu(recipes, null);
    }
}
