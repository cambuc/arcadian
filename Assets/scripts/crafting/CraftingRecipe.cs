using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CraftingRecipe : MonoBehaviour
{
    public Item product;
    public float timeInHours;
    public List<ItemQuantity> components = new List<ItemQuantity>();
    public List<ToolType> toolTypes = new List<ToolType>();
    public bool craftInWorld;

    public float timeCrafted { get; set; }

    public float TimeLeft()
    {
        return timeInHours - timeCrafted;
    }

    public int OrderByIndex()
    {
        bool hasComps = HasComponents();
        bool hasTools = HasTools();

        if (hasComps && hasTools) return 0;
        else if (hasComps || hasTools) return 1;
        else return 2;
    }

    public bool HasComponents()
    {
        craftInWorld = true;
        foreach (ItemQuantity comp in components)
        {
            if (!PlayerInventory.runtime.HasItemsSurroundings(comp.ToList()))
            {
                return false;
            }
        }
        return true;
    }
    public bool HasTools()
    {
        foreach (ToolType type in toolTypes)
        {
            bool hasTool = false;
            foreach (Tool tool in type.tools)
            {
                if (PlayerInventory.runtime.HasItemSurroundings(tool))
                {
                    hasTool = true;
                    break;
                }
            }
            if (!hasTool)
            {
                return false;
            }
        }
        return true;
    }

    public int QuantityCraftable()
    {
        int lowest = -1;
        List<Item> items = PlayerInventory.runtime.IncludeSurroundingItems();
        foreach (ItemQuantity comp in components)
        {
            int q = items.Where(i => i.itemName == comp.item.itemName).Count() / comp.amount;

            if (q < lowest || lowest == -1) lowest = q;
        }
        return lowest;
    }
    public virtual void CraftInWorld(int amount, CraftingStation station)
    {
        if(timeInHours <= 0)
        {
            PlayerInventory.runtime.AddItem(product);
            return;
        }

        for(int i = 0; i < amount; i++)
        {
            WorldItem world = product.Drop();
            UnfinishedProduct up = world.AddComponent<UnfinishedProduct>();
            up.Initialize(this, world);
        }
    }
}
[System.Serializable]
public class ItemQuantity
{
    public Item item;
    public int amount = 1;

    public List<Item> ToList()
    {
        List<Item> items = new List<Item>();
        for (int i = 0; i < amount; i++)
            items.Add(item);
        return items;
    }
}
