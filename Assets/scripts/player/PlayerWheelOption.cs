using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWheelOption : MonoBehaviour
{
    public Color normalColor;
    public Color highlightedColor;

    public Image image;

    public virtual void Open()
    {
    }

    public void Select()
    {
        image.color = highlightedColor;
    }
    public void Deselect()
    {
        image.color = normalColor;
    }
}
