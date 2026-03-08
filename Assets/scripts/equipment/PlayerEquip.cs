using System.Collections.Generic;
using UnityEngine;

public class PlayerEquip : MonoBehaviour
{
    public static PlayerEquip runtime;

    public Equippable current { get; set; }

    public Transform equipParent;

    public string unequipButton;

    private void Awake()
    {
        runtime = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(unequipButton))
            Unequip(current);
    }

    public bool IsEquipped(Equippable equippable)
    {
        return current != null && current.id == equippable.id;
    }
    public bool IsEquipped(Item item)
    {
        return current != null && current.id == item.id;
    }

    public void Equip(Equippable equippable, Item item)
    {
        Unequip(current);

        Equippable inst = Instantiate(equippable, equipParent).GetComponent<Equippable>();
        inst.id = item.id;
        current = inst;

        FirearmItem faItem = item.GetComponent<FirearmItem>();
        HuntingRifle faEquip = inst.GetComponent<HuntingRifle>();
        if(faItem && faEquip)
        {
            faEquip.magazine.count = faItem.loadedCount;
            faEquip.chambered = faItem.chambered;
            faEquip.item = faItem;
        }

        inst.Equip(() => { });
    }
    public void Unequip(Equippable equippable)
    {
        if (!current)
            return;

        Equippable holder = current;
        current.Unequip(() =>
        {
            Destroy(holder.gameObject);
        });
        current = null;
    }
}
