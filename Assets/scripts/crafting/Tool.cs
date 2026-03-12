using System.Collections.Generic;
using UnityEngine;

public class Tool : Item
{
    public float timeMultiplier;
    public float useConditionLoss;

    public AudioClip useSound;

    public void LoseCondition()
    {
        condition -= useConditionLoss;
        if(condition <= 0)
        {
            PlayerInventory.runtime.RemoveItem(id);
            HUDMessage.runtime.ShowMessage($"{itemName} broke");
            Destroy(this);
        }
    }
}
