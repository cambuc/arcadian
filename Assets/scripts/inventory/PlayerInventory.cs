using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework.Internal.Execution;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory runtime;
    public static UnityEvent onInventoryChanged = new UnityEvent();

    public WorldItem defaultWorldItem;

    public Transform player;
    public float surroundingsRadius;

    public int slots;
    public GameObject fullInventoryMessage;

    public List<Item> startingItems = new List<Item>();

    public Item grabbingItem { get; set; }

    List<Item> items = new List<Item>();
    public List<Item> GetItems()
    {
        return items;
    }

    public float GetTotalWeight()
    {
        float weight = 0;
        foreach(Item i in items)
        {
            weight += i.weight;
        }
        if (grabbingItem) weight += grabbingItem.weight;
        return weight;
    }

    public List<WorldItem> SurroundingObjects()
    {
        List<WorldItem> surItems = new List<WorldItem>();

        Collider[] cols = Physics.OverlapSphere(PlayerMovement.runtime.transform.position, surroundingsRadius, LayerMask.GetMask("Interactable"));
        foreach (Collider col in cols)
        {
            if (col.GetComponent<WorldItem>())
                surItems.Add(col.GetComponent<WorldItem>());
        }
        return surItems;
    }
    public List<Item> IncludeSurroundingItems()
    {
        List<Item> surItems = items.ToList();

        Collider[] cols = Physics.OverlapSphere(PlayerMovement.runtime.transform.position, surroundingsRadius, LayerMask.GetMask("Interactable"));
        foreach (Collider col in cols)
        {
            if (col.GetComponent<WorldItem>() && col.GetComponent<WorldItem>().item)
                surItems.Add(col.GetComponent<WorldItem>().item);
        }
        return surItems;
    }

    public bool HasItemSurroundings(Item item)
    {
        return IncludeSurroundingItems().Find(i => i.itemName == item.itemName);
    }
    public bool HasItem(Item item)
    {
        foreach(Item i in items)
        {
            if (i.itemName == item.itemName)
                return true;
        }
        return false;
    }
    public bool HasItemsSurroundings(List<Item> hasItems)
    {
        List<Item> invInst = IncludeSurroundingItems().ToList();
        foreach (Item item in hasItems)
        {
            Item inst = invInst.Find(i => i.itemName == item.itemName);
            if (inst)
            {
                invInst.Remove(item);
            }
            else
            {
                return false;
            }
        }
        return true;
    }
    public bool HasItems(List<Item> hasItems)
    {
        List<Item> invInst = items.ToList();
        foreach(Item item in hasItems)
        {
            Item inst = invInst.Find(i => i.itemName == item.itemName);
            if (inst)
            {
                invInst.Remove(item);
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    public int GetQuantity(Item item)
    {
        int q = 0;
        foreach(Item i in items)
        {
            if (i.itemName == item.itemName) q++;
        }
        return q;
    }
    public int GetQuantityIncludeSurroundings(Item item)
    {
        int q = 0;
        foreach (Item i in IncludeSurroundingItems())
        {
            if (i.itemName == item.itemName) q++;
        }
        return q;
    }

    private void Awake()
    {
        runtime = this;
    }

    private void Start()
    {
        foreach(Item item in startingItems)
            AddItem(item);
    }

    public int FilledSlots()
    {
        int slots = 0;
        foreach (Item i in items)
        {
            slots += i.dimensions.x * i.dimensions.y;
        }
        return slots;
    }

    public void AddItem(Item item)
    {
        Item newItem = item.Clone();
        newItem.NewId();
        items.Add(newItem);

        if (FilledSlots() > slots)
        {
            items.Find(i => i.itemName == newItem.itemName).Drop();
            ShowFullInventoryMessage();
        }
        if (newItem.GetComponent<FirearmItem>())
        {
            newItem.GetComponent<FirearmItem>().loadedCount = 0;
            newItem.GetComponent<FirearmItem>().chambered = false;
        }
        newItem.OnAdd();
        onInventoryChanged.Invoke();
    }
    async void ShowFullInventoryMessage()
    {
        fullInventoryMessage.SetActive(true);
        await Task.Delay(3000);
        fullInventoryMessage.SetActive(false);
    }

    public void RemoveItem(Item item)
    {
        Item toRemove = items.Find(i => i.itemName == item.itemName);
        if (toRemove)
            items.Remove(toRemove);
        onInventoryChanged.Invoke();
    }
    public void RemoveItem(Guid id)
    {
        Item toRemove = items.Find(i => i.id == id);
        if (toRemove)
            items.Remove(toRemove);
        onInventoryChanged.Invoke();
    }
    public void RemoveItemIncludeSurroundings(Item item)
    {
        Item toRemove = items.Find(i => i.itemName == item.itemName);
        if (toRemove)
            items.Remove(toRemove);
        else
        {
            foreach (WorldItem worldItem in SurroundingObjects())
            {
                if(worldItem.item.itemName == item.itemName && !worldItem.markedForDestruction)
                {
                    worldItem.markedForDestruction = true;
                    Destroy(worldItem.gameObject);
                    break;
                }
            }
        }
        onInventoryChanged.Invoke();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(player.position, surroundingsRadius);
    }
}
