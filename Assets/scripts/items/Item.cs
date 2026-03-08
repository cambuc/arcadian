using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public Guid id { get; private set; }

    public string itemName;
    public Texture2D icon;
    public WorldItem worldObject;
    public Equippable equippable;
    public float weight;
    public Vector2Int dimensions = Vector2Int.one;

    public void NewId()
    {
        id = Guid.NewGuid();
    }

    public int Volume()
    {
        return dimensions.x * dimensions.y;
    }

    public virtual void Drop()
    {
        WorldItem instance = Instantiate(worldObject == null ? PlayerInventory.runtime.defaultWorldItem.gameObject : worldObject.gameObject).GetComponent<WorldItem>();
        instance.SetItem(this);

        instance.transform.position = PlayerMovement.runtime.transform.position + Camera.main.transform.forward + Vector3.up;
        instance.name = itemName;

        Clothing asClothing = this as Clothing;
        if(asClothing && PlayerApparel.runtime.isWearing(asClothing)) PlayerApparel.runtime.RemoveClothing(asClothing);
        PlayerInventory.runtime.RemoveItem(this);

        if (equippable && PlayerEquip.runtime.current == equippable) PlayerEquip.runtime.Unequip(equippable);
    }

    public Item Clone()
    {
        return (Item)MemberwiseClone();
    }
}
