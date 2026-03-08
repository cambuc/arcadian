using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItem : Interactable
{
    public bool preventWorldFalling = true;
    public SoundPlayer optionClicked;

    public bool markedForDestruction { get; set; }

    private void Awake()
    {
        if (item == null)
            return;

        item = item.Clone();
        item.NewId();
    }

    private void OnEnable()
    {
        if (preventWorldFalling && !GetComponent<PreventWorldFalling>()) gameObject.AddComponent<PreventWorldFalling>();
    }

    public void SetItem(Item item)
    {
        this.item = item;
        interactName = item.itemName;
        item = item.Clone();
        item.NewId();
    }

    public override void Interact(string interactOption)
    {
        if (optionClicked) optionClicked.PlaySound();
        if (interactOption == "Store")
        {
            Store();
        }
        if (interactOption == "Equip")
        {
            Equip();
        }
        base.Interact(interactOption);
    }

    public void Store()
    {
        PlayerInventory.runtime.AddItem(item);
        if(GetType() != typeof(FlyingArrow))
            foreach (FlyingArrow arrow in GetComponentsInChildren<FlyingArrow>())
            {
                arrow.Interact("Store");
            }
        PlayerInteract.runtime.OnExitZone(() => Destroy(gameObject));
    }

    public void Equip()
    {
        PlayerEquip.runtime.Equip(item.equippable, item);
        Store();
    }
}
