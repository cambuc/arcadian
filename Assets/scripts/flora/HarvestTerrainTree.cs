using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HarvestTerrainTree : MonoBehaviour
{
    public TreeInstance treeInstance;

    public void Harvest()
    {
        List<TreeInstance> treeTemp = Terrain.activeTerrain.terrainData.treeInstances.ToList();
        treeTemp.Remove(treeInstance);
        Terrain.activeTerrain.terrainData.treeInstances = treeTemp.ToArray();
    }
}
