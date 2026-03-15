using System.Collections.Generic;
using UnityEngine;

public class FieldGuideSpecies : MonoBehaviour
{
    public string commonName;
    public string latinName;
    [TextArea(3, 15)]
    public string description;
    public Texture2D illustration;

    public bool discovered;
}
