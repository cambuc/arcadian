using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingMenu : MonoBehaviour
{
    public static CraftingMenu runtime;

    public GameObject root;
    public Button btnClose;

    public SoundPlayer selectSound;
    public CraftingStation currentStation { get; set; }

    [Header("Recipes List")]
    public Transform recipesLayout;
    public CraftingRecipeUI recipePrefab;
    public Scrollbar recipeScrollbar;

    [Header("Selection Window")]
    public GameObject selectionRoot;
    public TextMeshProUGUI txtProductName;
    public Transform componentsLayout;
    public CraftingComponentUI componentPrefab;
    public Transform toolTypesLayout;
    public CraftingToolTypeUI toolTypePrefab;
    public Counter quantityCounter;
    //public Counter timeCounter;
    public Button btnCraft;

    List<GameObject> tempRecipes = new List<GameObject>();
    List<GameObject> tempComps = new List<GameObject>();
    List<GameObject> tempTool = new List<GameObject>();

    bool passingTime;

    private void Awake()
    {
        runtime = this;

        btnClose.onClick.AddListener(CloseMenu);
    }

    private void Start()
    {
        CloseMenu();
    }

    public void OpenMenu(List<CraftingRecipe> recipes, CraftingStation station)
    {
        if (passingTime)
            return;

        currentStation = station;

        PlayerMovement.LockMovement(gameObject, true);
        FillRecipes(recipes);
        root.SetActive(true);

        recipeScrollbar.value = 1;
    }

    public void CloseMenu()
    {
        if (passingTime)
            return;

        root.SetActive(false);
        PlayerMovement.LockMovement(gameObject, false);
    }

    List<CraftingRecipe> current;
    void FillRecipes(List<CraftingRecipe> recipes)
    {
        ClearTempRecipes();
        current = recipes;
        foreach(CraftingRecipe recipe in recipes.OrderBy(r => r.OrderByIndex()))
        {
            CraftingRecipeUI inst = Instantiate(recipePrefab.gameObject, recipesLayout).GetComponent<CraftingRecipeUI>();
            inst.Initialize(recipe, recipe.HasTools() && recipe.HasComponents());
            inst.btnMain.onClick.AddListener(() => SelectRecipe(inst.recipe));
            tempRecipes.Add(inst.gameObject);
        }
        if (recipes.Count > 0) SelectRecipe(recipes[0]);
    }

    void SelectRecipe(CraftingRecipe recipe)
    {
        if (passingTime)
            return;

        selectSound.PlaySound();

        ClearTempComps();
        txtProductName.text = recipe.product.itemName;

        bool hasComps = true;
        bool hasToolTypes = true;

        foreach (ItemQuantity comp in recipe.components)
        {
            CraftingComponentUI inst = Instantiate(componentPrefab.gameObject, componentsLayout).GetComponent<CraftingComponentUI>();
            inst.Initialize(comp);

            if (!PlayerInventory.runtime.HasItemsSurroundings(comp.ToList()))
            {
                inst.txtMain.color = Color.red;
                hasComps = false;
            }

            tempComps.Add(inst.gameObject);
        }
        foreach (ToolType type in recipe.toolTypes)
        {
            CraftingToolTypeUI inst = Instantiate(toolTypePrefab.gameObject, toolTypesLayout).GetComponent<CraftingToolTypeUI>();
            inst.Initialize(type);

            bool hasTool = false;
            foreach (Tool tool in type.tools)
            {
                if (PlayerInventory.runtime.HasItemSurroundings(tool))
                {
                    hasTool = true;
                    break;
                }
            }
            if(!hasTool)
            {
                inst.txtMain.color = Color.red;
                hasToolTypes = false;
            }

            tempComps.Add(inst.gameObject);
        }
        if (recipe.toolTypes.Count <= 0)
        {
            CraftingToolTypeUI blank = Instantiate(toolTypePrefab.gameObject, toolTypesLayout).GetComponent<CraftingToolTypeUI>();
            blank.Initialize();
            tempComps.Add(blank.gameObject);
        }

        btnCraft.onClick.RemoveAllListeners();
        btnCraft.onClick.AddListener(() => CraftRecipe(recipe));

        btnCraft.interactable = hasComps && hasToolTypes;
        quantityCounter.SetInteractable(hasComps && hasToolTypes);

        quantityCounter.min = 1;
        quantityCounter.max = Mathf.Clamp(recipe.QuantityCraftable(), 0, 100);

        quantityCounter.onValueChange.AddListener(() =>
        {
            btnCraft.GetComponentInChildren<TextMeshProUGUI>().text = $"Craft ({GetTime(recipe, quantityCounter.value)} hrs)";
        });
        quantityCounter.SetValue(1);


        selectionRoot.SetActive(true);
    }

    float GetTime(CraftingRecipe recipe, float quantity)
    {
        return ((int)(Mathf.Sqrt(quantity) * recipe.timeInHours * 10)) / 10f;
    }

    public void CraftRecipe(CraftingRecipe recipe)
    {
        if (passingTime)
            return;

        passingTime = true;
        TimeManager.runtime.PassTime(GetTime(recipe, quantityCounter.value) * 60f * 60f);
        Fader.runtime.FadeOut(() => { OnFadeOut(recipe); });
    }
    void OnFadeOut(CraftingRecipe recipe)
    {
        for (int i = 0; i < quantityCounter.value; i++)
        {
            Item product = recipe.product.Clone();
            PlayerInventory.runtime.AddItem(product);
            product.NewId();
        }

        foreach (ItemQuantity comp in recipe.components)
        {
            for (int i = 0; i < comp.amount * quantityCounter.value; i++)
                PlayerInventory.runtime.RemoveItemIncludeSurroundings(comp.item);

        }
        FillRecipes(current);
        SelectRecipe(recipe);

        Fader.runtime.FadeIn(() => {
            passingTime = false;
            SelectRecipe(recipe);
        });
    }

    void ClearTempRecipes()
    {
        foreach(GameObject t in tempRecipes)
        {
            Destroy(t);
        }
        tempRecipes.Clear();
    }
    void ClearTempComps()
    {
        foreach (GameObject t in tempComps)
        {
            Destroy(t);
        }
        tempComps.Clear();
    }
}
