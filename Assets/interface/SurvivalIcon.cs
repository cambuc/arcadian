using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SurvivalIcon : MonoBehaviour
{
    public RawImage image;

    public TextMeshProUGUI lblStatus;

    [System.Serializable]
    public struct Status
    {
        public Texture2D icon;
        public string text;
    }
    public List<Status> statuses = new List<Status>();

    public enum Stat { exposure, thirst, hunger, fatigue, };
    public Stat stat;

    public Image fillColor;
    public Color cold;
    public Color hot;

    float recorded;

    private void Awake()
    {
        if(stat == Stat.exposure)
            GameTick.tick.AddListener(Exposure);
        else
            GameTick.tick.AddListener(Tick);

        recorded = -2;
    }

    void Tick()
    {
        float s = 0;
        if(stat == Stat.thirst) s = SurvivalAttributes.runtime.thirst;
        else if (stat == Stat.hunger) s = SurvivalAttributes.runtime.hunger;
        else s = SurvivalAttributes.runtime.fatigue;

        int i = (int)(s * 4);
        s = i / 4f;
        if (s != recorded)
        {
            recorded = s;
            //if (s != 0) HUDMessage.runtime.ShowMessage($"You are {statuses[i].text}");
            image.texture = statuses[i].icon;
            lblStatus.text = statuses[i].text;
        }
    }
    float lastTemp;
    void Exposure()
    {
        float s = SurvivalAttributes.runtime.temperature;

        if(lastTemp > s)
        {
            lastTemp = s;
            fillColor.color = cold;
        }
        else if (lastTemp < s)
        {
            lastTemp = s;
            fillColor.color = hot;
        }
        else
        {
            fillColor.color = Color.white;
        }

        if (s <= -0.25f) s = Mathf.Abs(s);
        else if (s >= 0.25f) s++;

        int i = (int)(s * 4);
        s = i / 4f;
        if (s != recorded)
        {
            recorded = s;
            //if (s != 0) HUDMessage.runtime.ShowMessage($"You are {statuses[i].text}");
            image.texture = statuses[i].icon;
            lblStatus.text = statuses[i].text;
        }
    }
}
