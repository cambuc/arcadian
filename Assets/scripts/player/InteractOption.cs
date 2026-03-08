using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct InteractOption
{
    public string text;

    public float interactTime;
    public bool saveTime;

    public ItemQuantity itemQuantity;
}
