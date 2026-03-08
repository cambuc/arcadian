using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CraftingToolTypeUI : MonoBehaviour
{
    public ToolType toolType { get; private set; }

    public TextMeshProUGUI txtMain;

    public void Initialize(ToolType toolType)
    {
        this.toolType = toolType;
        txtMain.text = toolType.name;
    }
    public void Initialize()
    {
        txtMain.text = "No Tools";
    }
}
