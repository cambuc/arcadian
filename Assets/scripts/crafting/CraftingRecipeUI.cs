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

    public GameObject selectionIndicator;

    [System.Serializable]
    public struct NameException
    {
        public CraftingRecipe recipe;
        public string name;
    }
    public List<NameException> exceptions = new List<NameException>();

    public void Initialize(CraftingRecipe recipe, bool craftable)
    {
        this.recipe = recipe;

        selectionIndicator.SetActive(false);

        txtTime.text = $"{recipe.timeInHours} hrs";
        txtProductName.color = craftable ? craftableColor : uncraftableColor;

        NameException except = exceptions.Find(e => e.recipe == recipe);
        if(except.recipe)
            txtProductName.text = except.name;
        else
            txtProductName.text = recipe.product.itemName;
    }

    public void Select()
    {
        selectionIndicator.SetActive(true);
    }
    public void Deselect()
    {
        selectionIndicator.SetActive(false);
    }
}
