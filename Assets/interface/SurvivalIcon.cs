using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SurvivalIcon : MonoBehaviour
{
    public RawImage image;
    public List<Texture2D> icons = new List<Texture2D>();

    public enum Stat { thirst, hunger, fatigue, };
    public Stat stat;

    private void Awake()
    {
        GameTick.tick.AddListener(Tick);
    }

    void Tick()
    {
        float s = 0;
        if(stat == Stat.thirst) s = SurvivalAttributes.runtime.thirst;
        else if (stat == Stat.hunger) s = SurvivalAttributes.runtime.hunger;
        else s = SurvivalAttributes.runtime.fatigue;

        if (s >= 1) image.texture = icons[4];
        else if (s >= 0.75f) image.texture = icons[3];
        else if (s >= 0.5f) image.texture = icons[2];
        else if (s >= 0.25f) image.texture = icons[1];
        else image.texture = icons[0];
    }
}
