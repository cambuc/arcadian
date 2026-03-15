using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Container : Interactable
{
    public List<Item> items = new List<Item>();
    public SoundPlayer optionSound;

    public float GetWeight()
    {
        float weight = 0;
        foreach (Item i in items) weight += i.weight;
        return weight;
    }
    public override void SetInteractOptions()
    {
        interactOptions.Clear();

        interactOptions.Add(new InteractOption()
        {
            text = "Store"
        });
        interactOptions.Add(new InteractOption()
        {
            text = "Grab"
        });

        if (item) item.weight = GetWeight();

        if (items.Count <= 0) return;

        items = items.OrderBy(i => i.itemName).ToList();
        Item current = items[0];
        int q = 0;
        foreach (Item item in items)
        {
            if(current.itemName == item.itemName)
            {
                q++;
            }
            else
            {
                interactOptions.Add(BuildInteractOption(current, q));
                q = 1;
                current = item;
            }
        }
        interactOptions.Add(BuildInteractOption(current, q));
    }
    InteractOption BuildInteractOption(Item item, int quantity)
    {
        return new InteractOption()
        {
            text = $"Take {item.itemName} ({quantity})",
            itemQuantity = new ItemQuantity() { item = item, amount = quantity },
        };
    }

    public override void Interact(string interactOption)
    {
        

        optionSound.PlaySound();

        if(interactOption == "Store")
        {
            PlayerInventoryUI.runtime.OpenSelectionInventory(StoreItem);
            return;
        }
        if (interactOption == "Grab")
        {
            Grab();
            return;
        }

        Item remove = null;
        foreach(Item item in items)
        {
            if (interactOption.Contains(item.itemName))
            {
                PlayerInventory.runtime.AddItem(item);
                remove = item;
                break;
            }
        }
        if (remove)
        {
            items.Remove(remove);
            SetInteractOptions();
        }
    }

    public void StoreItem(Item item)
    {
        if (item == null) return;

        items.Add(item);
        PlayerInventory.runtime.RemoveItem(item);
        SetInteractOptions();
    }
}
