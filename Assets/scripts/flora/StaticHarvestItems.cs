using System.Collections.Generic;
using UnityEngine;

public class StaticHarvestItems : Interactable
{
    public string id;

    public List<HarvestItem> harvestItems = new List<HarvestItem>();

    public override void SetInteractOptions()
    {
        for (int i = 0; i < harvestItems.Count; i++)
        {
            HarvestItem hi = harvestItems[i];
            hi.assignedCount = Random.Range(hi.randomCountRange.x, hi.randomCountRange.y + 1);
            harvestItems[i] = hi;
            if (hi.assignedCount > 0 || hi.infinite || hi.destroyObject)
                interactOptions.Add(BuildInteractOption(hi));
        }
        if (GetComponent<TreeFeller>())
            interactOptions.Add(new InteractOption() { text = "Fell", interactTime = 0.5f });
    }

    public override void Interact(string interactOption)
    {
        

        if (interactOption == "Fell")
        {
            GetComponent<TreeFeller>().Interact();
            return;
        }

        int harvested = -1;
        for (int i = 0; i < harvestItems.Count; i++)
        {
            if(interactOption == GetOptionText(harvestItems[i]))
            {
                harvested = i;
                PlayerInventory.runtime.AddItem(harvestItems[i].item);
                if(harvestItems[harvested].onHarvest) harvestItems[harvested].onHarvest.PlaySound();
            }
        }
        if (harvested >= 0)
        {
            if (harvestItems[harvested].infinite) return;

            if (harvestItems[harvested].destroyObject)
            {
                if (GetComponent<HarvestTerrainTree>())
                    GetComponent<HarvestTerrainTree>().Harvest();
                Destroy(gameObject);
                return;
            }

            HarvestItem reduced = harvestItems[harvested];
            reduced.assignedCount--;
            harvestItems[harvested] = reduced;

            if (reduced.assignedCount <= 0)
            {
                harvestItems.RemoveAt(harvested);
                interactOptions.Remove(interactOptions.Find(i => i.text == interactOption));

                if(interactOptions.Count <= 0)
                    Destroy(gameObject);

                return;
            }

            for (int i = 0; i < interactOptions.Count; i++)
            {
                if (interactOptions[i].itemQuantity != null && interactOptions[i].itemQuantity.item == harvestItems[harvested].item)
                {
                    interactOptions[i] = BuildInteractOption(harvestItems[harvested]);
                }
            }
        }
    }

    InteractOption BuildInteractOption(HarvestItem hi)
    {
        return new InteractOption()
        {
            text = GetOptionText(hi),
            itemQuantity = new ItemQuantity() { item = hi.item, amount = hi.assignedCount },
            interactTime = hi.interactTime,
            saveTime = hi.saveTime
        };
    }

    string GetOptionText(HarvestItem hi)
    {
        if(hi.infinite || hi.destroyObject)
            return $"Harvest {hi.item.itemName}";
        else
            return $"Harvest {hi.item.itemName} ({hi.assignedCount})";
    }
}
[System.Serializable]
public struct HarvestItem
{
    public Item item;
    public Vector2Int randomCountRange;
    [HideInInspector]
    public int assignedCount;
    public float interactTime;
    public bool saveTime;
    public bool destroyObject;
    public bool infinite;
    public SoundPlayer onHarvest;
}
