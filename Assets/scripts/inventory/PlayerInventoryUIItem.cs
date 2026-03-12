using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventoryUIItem : MonoBehaviour
{
    public Item item { get; set; }

    public Button button;
    public RawImage icon;
    public TextMeshProUGUI txtName;
    public Slider condition;

    public void Initiate()
    {
        item = null;
        txtName.text = "";
        icon.enabled = false;
        condition.gameObject.SetActive(false);
    }
    public void Initiate(Item item)
    {
        this.item = item;
        txtName.text = item.itemName;
        icon.enabled = item.icon;
        icon.texture = item.icon;
        condition.gameObject.SetActive(item.hasCondition);
        condition.value = item.condition;

        if (item as Clothing && PlayerApparel.runtime.isWearing(item as Clothing)) return;

        RectTransform rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x * item.dimensions.x, rect.sizeDelta.y * item.dimensions.y);
    }
}
