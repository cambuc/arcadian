using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backpack : Container
{
    public Clothing.Type type;

    public override void Interact(string interactOption)
    {
        

        if (interactOption == "Wear")
        {
            WearPack();
            return;
        }

        base.Interact(interactOption);
    }

    public void WearPack()
    {
        PlayerInteract.runtime.OnExitZone();

        GameObject i = Instantiate(item.gameObject);
        i.GetComponent<ClothingBackpack>().items = items;
        PlayerApparel.runtime.WearClothing(i.GetComponent<ClothingBackpack>());

        Destroy(gameObject);
    }
}
