using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : Interactable
{
    public List<Item> items = new List<Item>();
    public float volume;

    public float GetWeight()
    {
        float weight = 0;
        foreach (Item i in items) weight += i.weight;
        return weight;
    }
}
