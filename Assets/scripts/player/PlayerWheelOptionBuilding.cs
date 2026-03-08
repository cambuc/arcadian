using System.Collections.Generic;
using UnityEngine;

public class PlayerWheelOptionBuilding : PlayerWheelOption
{
    public override void Open()
    {
        BuildingMenu.runtime.OpenMenu();
    }
}
