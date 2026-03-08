using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerApparelUI : MonoBehaviour
{
    public static PlayerApparelUI runtime;

    public PlayerInventoryUIItem itemHeadwear;
    public PlayerInventoryUIItem itemJacket2;
    public PlayerInventoryUIItem itemJacket1;
    public PlayerInventoryUIItem itemShirt2;
    public PlayerInventoryUIItem itemShirt1;
    public PlayerInventoryUIItem itemUnderwear;
    public PlayerInventoryUIItem itemSocks;
    public PlayerInventoryUIItem itemShoes;
    public PlayerInventoryUIItem itemBack;

    private void Awake()
    {
        runtime = this;
    }

    public void WearClothing(Clothing c)
    {
        if(c.type == Clothing.Type.Headwear)
        {
            
        }
    }
    public void RemoveClothing(Clothing c)
    {

    }
}
