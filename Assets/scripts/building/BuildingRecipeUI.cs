using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuildingRecipeUI : MonoBehaviour
{
    public BuildingRecipe recipe { get; set; }

    public GameObject selection;
    public TextMeshProUGUI txtMain;

    public bool buildable { get; set; }

    public void Initialize(BuildingRecipe recipe)
    {
        this.recipe = recipe;

        txtMain.text = recipe.gameObject.name;
        selection.gameObject.SetActive(false);
    }

    public void Select()
    {
        selection.gameObject.SetActive(true);
    }
    public void Deselect()
    {
        selection.gameObject.SetActive(false);
    }
}
