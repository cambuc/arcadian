using System.Collections.Generic;
using UnityEngine;

public class PlayerWheelOptionFieldGuide : PlayerWheelOption
{
    public override void Open()
    {
        FieldGuide.runtime.OpenGuide();
    }
}
