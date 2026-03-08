using System.Collections.Generic;
using UnityEngine;

public class HuntingRifleControls : MonoBehaviour
{
    public static HuntingRifleControls runtime;
    public GameObject root;

    private void Awake()
    {
        runtime = this;
        root.SetActive(false);
    }
}
