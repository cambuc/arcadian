using System.Collections.Generic;
using UnityEngine;

public class LoadTerrain : MonoBehaviour
{
    public TerrainTreePainter treePainter;
    public TerrainDetailPainter detailPainter;
    public TerrainTreeSwapper treeColliders;
    public List<TerrainDetailSwapper> detailColliders = new List<TerrainDetailSwapper>();

    private void Start()
    {
#if UNITY_EDITOR
        return;
#endif

        treePainter.PaintTrees();
        detailPainter.PaintDetails();
        treeColliders.GenerateColliders();
        foreach(TerrainDetailSwapper d in detailColliders)
        {
            d.GenerateColliders();
        }
    }
}
