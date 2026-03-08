using System.Collections.Generic;
using UnityEngine;

public class PlayerWheelOptionInventory : PlayerWheelOption
{
    public override void Open()
    {
        PlayerInventoryUI.runtime.OpenInventory();
    }
}
