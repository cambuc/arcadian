using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CraftingRecipeUI : MonoBehaviour
{
    public CraftingRecipe recipe { get; set; }

    public TextMeshProUGUI txtProductName;
    public TextMeshProUGUI txtTime;
    public Button btnMain;

    public Color craftableColor;
    public Color uncraftableColor;

    public void Initialize(CraftingRecipe recipe, bool craftable)
    {
        this.recipe = recipe;
        txtProductName.text = recipe.product.itemName;
        txtTime.text = "" + recipe.timeInHours;
        txtProductName.color = craftable ? craftableColor : uncraftableColor;
    }
}
