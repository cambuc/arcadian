using System.Collections.Generic;
using UnityEngine;

public class CookingRecipe : CraftingRecipe
{
    public bool drying;

    public override void CraftInWorld(int amount, CraftingStation station)
    {
        for (int i = 0; i < amount; i++)
            station.GetComponent<Campfire>().PlaceMeatCooking(this);
    }
}
