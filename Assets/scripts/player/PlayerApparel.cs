using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerApparel : MonoBehaviour
{
    public static PlayerApparel runtime;

    public Vector2Int baseIdealTemperatureRange;
    public Vector2Int idealTemperatureRange { get; set; }

    public List<Clothing> apparel = new List<Clothing>();
    public List<Clothing> wornApparel = new List<Clothing>();

    private void Awake()
    {
        runtime = this;

        idealTemperatureRange = baseIdealTemperatureRange;
    }

    public bool isWearing(Clothing clothing)
    {
        foreach (Clothing worn in wornApparel)
        {
            if (clothing.id == worn.id)
                return true;
        }
        return false;
    }

    public void WearClothing(Clothing clothing)
    {
        Clothing toRemove = null;
        foreach(Clothing c in wornApparel)
        {
            if(c.type == clothing.type)
            {
                toRemove = c;
                break;
            }
        }
        if (toRemove)
        {
            wornApparel.Remove(toRemove);
            idealTemperatureRange += new Vector2Int(toRemove.coldResist, toRemove.coldResist);
        }

        wornApparel.Add(clothing);
        idealTemperatureRange -= new Vector2Int(clothing.coldResist, clothing.coldResist);
    }

    public void RemoveClothing(Clothing clothing)
    {
        wornApparel.Remove(clothing);
        idealTemperatureRange += new Vector2Int(clothing.coldResist, clothing.coldResist);
    }
}
