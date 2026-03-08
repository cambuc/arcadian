using System.Collections.Generic;
using UnityEngine;

public class ForageItem : WorldItem
{
    public Item forageItem;
    public SoundPlayer onHarvest;

    public override void Interact(string interactOption)
    {
        if (interactOption == "Harvest")
        {
            if (onHarvest) onHarvest.PlaySound();
            Harvest();
        }
        base.Interact(interactOption);
    }
    void Harvest()
    {
        PlayerInventory.runtime.AddItem(forageItem);
        if (GetType() != typeof(FlyingArrow))
            foreach (FlyingArrow arrow in GetComponentsInChildren<FlyingArrow>())
            {
                arrow.Interact("Store");
            }
        Destroy(gameObject);
    }
}
