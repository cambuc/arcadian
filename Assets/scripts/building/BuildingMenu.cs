using System.Collections.Generic;
using UnityEngine;

public class BuildingMenu : MonoBehaviour
{
    public static BuildingMenu runtime;

    public bool groundPreview { get; set; }

    public List<BuildingRecipe> recipes = new List<BuildingRecipe>();
    public string openKey;

    public Terrain terrain;
    public int detailLayerCount;

    public SoundPlayer scrollSound;
    public SoundPlayer buildSound;

    [Header("Preview")]
    public Vector3 previewOffset;
    public float rotationSpeed;

    [Header("UI Components")]
    public GameObject root;
    public Transform recipesLayout;
    public BuildingRecipeUI recipePrefab;

    [Header("Components")]
    public CraftingComponentUI compPrefab;
    public GameObject compRoot;

    int selectionIndex;
    BuildingPreview preview;
    List<BuildingRecipeUI> selections = new List<BuildingRecipeUI>();

    List<GameObject> tempComps = new List<GameObject>();

    private void Awake()
    {
        runtime = this;

        groundPreview = true;
    }

    private void Start()
    {
        PlayerMovement.LockMovement(gameObject, false);

        selectionIndex = 0;

        CloseMenu();
    }

    private void Update()
    {
        if (PlayerMovement.IsMovementLocked())
            return;

        if (Input.GetKeyDown(openKey) && !root.activeSelf)
        {
            OpenMenu();
        }
        if ((Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab)) && root.activeSelf)
        {
            CloseMenu();
        }

        if (!root.activeSelf)
            return;

        if ((Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.A)) ||
            Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SelectRecipe(selectionIndex - 1);
        }
        if ((Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.D)) ||
            Input.GetKeyDown(KeyCode.RightArrow))
        {
            SelectRecipe(selectionIndex + 1);
        }

        if (Input.GetKeyDown(KeyCode.E) && selections[selectionIndex].buildable)
        {
            PlaceBuilding();
        }

        if (Input.GetKeyDown(KeyCode.G))
            groundPreview = !groundPreview;
    }

    public void OpenMenu()
    {
        FillLayout();
        root.SetActive(true);
    }
    public void CloseMenu()
    {
        if (preview) Destroy(preview.gameObject);

        root.SetActive(false);
    }

    void FillLayout()
    {
        ClearTemp();

        if (recipes.Count <= 0)
            return;

        for (int i = 0; i < recipes.Count; i++)
        {
            BuildingRecipeUI inst = Instantiate(recipePrefab.gameObject, recipesLayout).GetComponent<BuildingRecipeUI>();
            inst.Initialize(recipes[i]);
            selections.Add(inst);
        }
        SelectRecipe(selectionIndex);
    }
    void ClearTemp()
    {
        foreach(BuildingRecipeUI t in selections)
        {
            Destroy(t.gameObject);
        }
        selections.Clear();
    }

    void SelectRecipe(int index)
    {
        if (index >= selections.Count || index < 0)
            return;

        ClearComps();

        scrollSound.PlaySound();

        if(selectionIndex < selections.Count && selectionIndex >= 0) selections[selectionIndex].Deselect();

        selectionIndex = index;
        selections[selectionIndex].Select();

        if (preview) Destroy(preview.gameObject);
        preview = Instantiate(selections[selectionIndex].recipe.buildingPrefab, Camera.main.transform).AddComponent<BuildingPreview>();
        preview.transform.localPosition = previewOffset;
        preview.SetValues(selections[selectionIndex].recipe, rotationSpeed);

        selections[selectionIndex].buildable = true;
        BuildingRecipe recipe = selections[selectionIndex].recipe;
        foreach (ItemQuantity comp in recipe.components)
        {
            CraftingComponentUI inst = Instantiate(compPrefab.gameObject, compRoot.transform).GetComponent<CraftingComponentUI>();
            inst.Initialize(comp);

            if (!PlayerInventory.runtime.HasItemsSurroundings(comp.ToList()))
            {
                inst.txtMain.color = Color.red;
                selections[selectionIndex].buildable = false;
            }

            tempComps.Add(inst.gameObject);
        }
    }

    void ClearComps()
    {
        foreach(GameObject c in tempComps)
        {
            Destroy(c);
        }
        tempComps.Clear();
    }

    void PlaceBuilding()
    {
        buildSound.PlaySound();
        PlayerMovement.LockMovement(gameObject, true);
        Cursor.visible = false;
        Fader.runtime.FadeOut(OnFadeOut);

        foreach(ItemQuantity iq in selections[selectionIndex].recipe.components)
            for(int i = 0; i < iq.amount; i++)
                PlayerInventory.runtime.RemoveItemIncludeSurroundings(iq.item);
    }
    void OnFadeOut()
    {
        TimeManager.runtime.PassTime(preview.recipe.timeInHours * 60f * 60f);

        int cx = (int)(preview.transform.position.z * (terrain.terrainData.detailResolution / terrain.terrainData.size.z));
        int cy = (int)(preview.transform.position.x * (terrain.terrainData.detailResolution / terrain.terrainData.size.x));

        for (int i = 0; i < detailLayerCount; i++)
        {
            int[,] layer = terrain.terrainData.GetDetailLayer(0, 0, terrain.terrainData.detailResolution, terrain.terrainData.detailResolution , i);

            for(int x = cx - preview.recipe.radius; x < cx + preview.recipe.radius; x++)
                for (int y = cy - preview.recipe.radius; y < cy + preview.recipe.radius; y++)
                    layer[x, y] = 0;

            terrain.terrainData.SetDetailLayer(0, 0, i, layer);
        }

        preview.Place();
        Destroy(preview);
        preview = null;
        SelectRecipe(selectionIndex);

        CloseMenu();
        Fader.runtime.FadeIn();
        PlayerMovement.LockMovement(gameObject, false);
    }
}
