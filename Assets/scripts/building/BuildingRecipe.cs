using System.Collections.Generic;
using UnityEngine;

public class BuildingRecipe : MonoBehaviour
{
    public GameObject buildingPrefab;
    public float timeInHours;
    public int radius;
    public List<ItemQuantity> components = new List<ItemQuantity>();
    public List<ToolType> toolTypes = new List<ToolType>();

    public float groundHeightOffset;
}
