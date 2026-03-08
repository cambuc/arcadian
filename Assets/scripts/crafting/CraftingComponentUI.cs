using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CraftingComponentUI : MonoBehaviour
{
    public TextMeshProUGUI txtMain;
    public ItemQuantity iq { get; private set; }

    public void Initialize(ItemQuantity iq)
    {
        this.iq = iq;
        txtMain.text = $"{iq.item.itemName}\n{PlayerInventory.runtime.GetQuantityIncludeSurroundings(iq.item)}/{iq.amount}";
    }
}
