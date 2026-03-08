using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    public RectTransform image;
    public float offset;

    private void Awake()
    {
        GameTick.tick.AddListener(Tick);
    }

    void Tick()
    {
        float z = TimeManager.runtime.currentDateTime.Hour * (360f / 24f) + TimeManager.runtime.currentDateTime.Minute * (360f / (24f * 60f)) + offset;
        image.eulerAngles = new Vector3(image.eulerAngles.x, image.eulerAngles.y, z);
    }
}
