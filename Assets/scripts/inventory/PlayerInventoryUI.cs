using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerInventoryUI : MonoBehaviour
{
    public static PlayerInventoryUI runtime;
    public static UnityEvent<Item> onClose = new UnityEvent<Item>();

    public PlayerInventoryUIItem prefab;
    public string openKey = "i";

    [Header("Sounds")]
    public SoundPlayer selectItemSound;
    public SoundPlayer clickSound;

    [Header("UI Components")]
    public GameObject root;
    public ItemsLayout itemsLayoutRoot;
    public Button btnClose;
    public GameObject missingItemsIndicator;
    public TextMeshProUGUI lblWeight;
    public TextMeshProUGUI lblSlots;

    [Header("Context Menu")]
    public GameObject contextRoot;
    public Button btnWear;
    public Button btnRemove;
    public Button btnEquip;
    public Button btnConsume;
    public Button btnDrop;
    public TextMeshProUGUI txtItemName;
    public TextMeshProUGUI lblItemWeight;
    public TextMeshProUGUI lblItemSlots;
    public TextMeshProUGUI lblItemCondition;

    [Header("Apparel Slots")]
    public GameObject apparelRoot;
    public List<PlayerInventoryUIItem> apparelItems = new List<PlayerInventoryUIItem>();

    PlayerInventoryUIItem current;

    List<GameObject> temp = new List<GameObject>();

    private void Awake()
    {
        runtime = this;

        btnClose.onClick.AddListener(CloseInventory);
    }

    private void Start()
    {
        CloseInventory();
    }

    private void Update()
    {
        if (Input.GetKeyDown(openKey) && !root.activeSelf && !PlayerMovement.IsMovementLocked() && !BuildingMenu.runtime.root.activeSelf)
        {
            OpenInventory();
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            RaycastResult rr = results.Find(r => r.gameObject.GetComponent<PlayerInventoryUIItem>());
            if (rr.gameObject)
            {
                DropItem(rr.gameObject.GetComponent<PlayerInventoryUIItem>());
                clickSound.PlaySound();
            }
        }
    }

    public void OpenInventory()
    {
        PlayerMovement.LockMovement(gameObject, true);

        contextRoot.SetActive(true);
        apparelRoot.SetActive(true);

        txtItemName.text = "";
        lblItemWeight.text = "";
        lblItemSlots.text = "";
        lblItemCondition.text = "";

        btnWear.gameObject.SetActive(false);
        btnRemove.gameObject.SetActive(false);
        btnEquip.gameObject.SetActive(false);
        btnConsume.gameObject.SetActive(false);
        btnDrop.gameObject.SetActive(false);

        FillLayout();
        root.SetActive(true);
    }

    public void CloseInventory()
    {
        onClose.Invoke(current ? current.item : null);
        onClose.RemoveAllListeners();

        ClearTemp();

        root.SetActive(false);
        PlayerMovement.LockMovement(gameObject, false);
    }

    void FillLayout()
    {
        ClearTemp();

        lblWeight.text = $"{(int)(10f * PlayerInventory.runtime.GetTotalWeight()) / 10f} lbs";
        lblSlots.text = $"{100 * PlayerInventory.runtime.FilledSlots() / PlayerInventory.runtime.slots}% full";

        missingItemsIndicator.SetActive(false);

        foreach (PlayerInventoryUIItem item in apparelItems)
        {
            item.button.onClick.RemoveAllListeners();
            item.Initiate();
        }
        foreach (Clothing worn in PlayerApparel.runtime.wornApparel)
        {
            apparelItems[(int)worn.type].Initiate(worn);
            apparelItems[(int)worn.type].button.onClick.AddListener(() => SelectItem(apparelItems[(int)worn.type]));
        }

        List<PlayerInventoryUIItem> uiItems = new List<PlayerInventoryUIItem>();
        foreach (Item item in PlayerInventory.runtime.GetItems())
        {
            if (item.GetType() == typeof(Clothing) && PlayerApparel.runtime.isWearing((Clothing)item))
                continue;

            GameObject instance = Instantiate(prefab.gameObject, itemsLayoutRoot.transform);
            temp.Add(instance);

            PlayerInventoryUIItem uiItem = instance.GetComponent<PlayerInventoryUIItem>();
            uiItems.Add(uiItem);

            uiItem.Initiate(item);
            uiItem.button.onClick.AddListener(() => SelectItem(uiItem));
        }

        //for(int i = 0; i < spaceLeft + PlayerApparel.runtime.wornApparel.Count; i++)
        //{
        //    GameObject instance = Instantiate(prefab.gameObject, itemsLayoutRoot.transform);
        //    temp.Add(instance);

        //    PlayerInventoryUIItem uiItem = instance.GetComponent<PlayerInventoryUIItem>();
        //    uiItem.Initiate();
        //    uiItem.button.onClick.AddListener(() => SelectItem(null));
        //}
        if (PlayerInventory.runtime.GetItems().Count > 0) itemsLayoutRoot.AdjustLayout(uiItems);
    }

    //Inventory Selection--------------------------------------
    public void OpenSelectionInventory(UnityAction<Item> onSelect)
    {
        onClose.AddListener(onSelect);

        PlayerMovement.LockMovement(gameObject, true);

        apparelRoot.SetActive(false);
        contextRoot.SetActive(false);

        FillLayoutSelection();
        root.SetActive(true);
    }
    void FillLayoutSelection()
    {
        ClearTemp();

        lblWeight.text = "";
        lblSlots.text = "";

        missingItemsIndicator.SetActive(false);
        List<PlayerInventoryUIItem> uiItems = new List<PlayerInventoryUIItem>();
        foreach (Item item in PlayerInventory.runtime.GetItems())
        {
            GameObject instance = Instantiate(prefab.gameObject, itemsLayoutRoot.transform);
            temp.Add(instance);

            PlayerInventoryUIItem uiItem = instance.GetComponent<PlayerInventoryUIItem>();
            uiItems.Add(uiItem);
            uiItem.Initiate(item);
            uiItem.button.onClick.AddListener(() => {
                clickSound.PlaySound();
                current = uiItem;
                CloseInventory();
            });
        }
        if (PlayerInventory.runtime.GetItems().Count > 0) itemsLayoutRoot.AdjustLayout(uiItems);
    }

    //Filtered Inventory Selection--------------------------------------
    public void OpenFilteredInventory(List<Item> includes, UnityAction<Item> onSelect)
    {
        onClose.AddListener(onSelect);

        PlayerMovement.LockMovement(gameObject, true);

        apparelRoot.SetActive(false);
        contextRoot.SetActive(false);

        FillLayoutSelection(includes);
        root.SetActive(true);
    }
    void FillLayoutSelection(List<Item> includes)
    {
        ClearTemp();

        lblWeight.text = "";
        lblSlots.text = "";

        missingItemsIndicator.SetActive(false);
        List<PlayerInventoryUIItem> uiItems = new List<PlayerInventoryUIItem>();
        foreach (Item item in PlayerInventory.runtime.GetItems())
        {
            if (!includes.Find(i => i.itemName == item.itemName))
                continue;

            GameObject instance = Instantiate(prefab.gameObject, itemsLayoutRoot.transform);
            temp.Add(instance);

            PlayerInventoryUIItem uiItem = instance.GetComponent<PlayerInventoryUIItem>();
            uiItems.Add(uiItem);
            uiItem.Initiate(item);
            uiItem.button.onClick.AddListener(() => {
                clickSound.PlaySound();
                current = uiItem;
                CloseInventory();
            });
        }
        if (uiItems.Count <= 0) missingItemsIndicator.SetActive(true);
        if (PlayerInventory.runtime.GetItems().Count > 0) itemsLayoutRoot.AdjustLayout(uiItems);
    }

    public void Refresh()
    {
        SelectItem(current);
    }

    void SelectItem(PlayerInventoryUIItem uiItem)
    {
        current = uiItem;
        txtItemName.text = "";
        lblItemWeight.text = "";
        lblItemSlots.text = "";
        lblItemCondition.text = "";

        btnWear.gameObject.SetActive(false);
        btnWear.onClick.RemoveAllListeners();

        btnRemove.gameObject.SetActive(false);
        btnRemove.onClick.RemoveAllListeners();

        btnEquip.gameObject.SetActive(false);
        btnEquip.onClick.RemoveAllListeners();

        btnConsume.gameObject.SetActive(false);
        btnConsume.onClick.RemoveAllListeners();

        btnDrop.gameObject.SetActive(false);
        btnDrop.onClick.RemoveAllListeners();

        if (uiItem == null || uiItem.item == null)
            return;

        selectItemSound.PlaySound();
        uiItem.button.Select();

        txtItemName.text = $"{uiItem.item.itemName}";
        lblItemWeight.text = $"{uiItem.item.weight} lbs";
        lblItemSlots.text = $"{uiItem.item.dimensions.x}x{uiItem.item.dimensions.y}";
        if(uiItem.item.hasCondition) lblItemCondition.text = $"{(int)(uiItem.item.condition * 100)}%";

        btnDrop.gameObject.SetActive(true);
        btnDrop.onClick.AddListener(() => DropItem(uiItem));

        if (uiItem.item.equippable)
        {
            btnEquip.gameObject.SetActive(true);
            if(!PlayerEquip.runtime.IsEquipped(uiItem.item))
            {
                btnEquip.onClick.AddListener(() => EquipItem(uiItem));
                btnEquip.GetComponentInChildren<TextMeshProUGUI>().text = "Equip";
            }
            else
            {
                btnEquip.onClick.AddListener(() => UnequipItem(uiItem));
                btnEquip.GetComponentInChildren<TextMeshProUGUI>().text = "Unequip";
            }
        }

        if (uiItem.item.GetType() == typeof(Clothing))
        {
            Clothing c = (Clothing)uiItem.item;

            if (PlayerApparel.runtime.isWearing(c))
            {
                btnRemove.gameObject.SetActive(true);
                btnRemove.onClick.AddListener(() => RemoveClothing(uiItem));
            }
            else
            {
                btnWear.gameObject.SetActive(true);
                btnWear.onClick.AddListener(() => WearClothing(uiItem));
            }
        }
        if (uiItem.item.GetType() == typeof(Consumable))
        {
            btnConsume.gameObject.SetActive(true);
            btnConsume.onClick.AddListener(() => ConsumeItem(uiItem));
        }
    }

    void ConsumeItem(PlayerInventoryUIItem uiItem)
    {
        ((Consumable)uiItem.item).OnConsume();
        SelectItem(null);
        FillLayout();
    }

    void WearClothing(PlayerInventoryUIItem uiItem)
    {
        PlayerApparel.runtime.WearClothing((Clothing)uiItem.item);
        SelectItem(null);
        FillLayout();
    }
    void RemoveClothing(PlayerInventoryUIItem uiItem)
    {
        PlayerApparel.runtime.RemoveClothing((Clothing)uiItem.item);
        SelectItem(null);
        FillLayout();
    }

    void DropItem(PlayerInventoryUIItem uiItem)
    {
        uiItem.item.Drop();
        SelectItem(null);
        FillLayout();
    }

    void EquipItem(PlayerInventoryUIItem uiItem)
    {
        PlayerEquip.runtime.Equip(uiItem.item.equippable, uiItem.item);
        SelectItem(uiItem);
    }
    void UnequipItem(PlayerInventoryUIItem uiItem)
    {
        PlayerEquip.runtime.Unequip(uiItem.item.equippable);
        SelectItem(uiItem);
    }

    void ClearTemp()
    {
        foreach(GameObject gm in temp)
        {
            Destroy(gm);
        }
        temp.Clear();
    }
}
