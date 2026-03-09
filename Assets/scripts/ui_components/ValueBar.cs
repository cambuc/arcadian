using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ValueBar : MonoBehaviour
{
    public RectTransform fill;

    float value01;
    public float Value
    {
        get { return value01; }
        set
        {
            value01 = Mathf.Clamp01(value);
            UpdateFill();
        }
    }

    void UpdateFill()
    {
        RectTransform rt = GetComponent<RectTransform>();
        fill.sizeDelta = new Vector2(Mathf.Lerp(0, rt.rect.width, value01), fill.sizeDelta.y);
        fill.localPosition = new Vector3(Mathf.Lerp(-rt.rect.width / 2, 0, value01), fill.localPosition.y, fill.localPosition.z);
    }
}
