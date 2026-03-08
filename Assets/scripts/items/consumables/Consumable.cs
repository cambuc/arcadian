using UnityEngine;

public class Consumable : Item
{
    public float sustenance;

    public virtual void OnConsume()
    {
        SurvivalAttributes.runtime.AddHunger(-sustenance);
        PlayerInventory.runtime.RemoveItem(this);
    }
}
