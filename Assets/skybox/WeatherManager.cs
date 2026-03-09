using System.Collections.Generic;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    public List<GameObject> clouds = new List<GameObject>();

    private void Start()
    {
        foreach(GameObject cloud in clouds)
        {
            if (Random.Range(0, 2) == 1) cloud.SetActive(true);
            else cloud.SetActive(false);
        }
    }
}
