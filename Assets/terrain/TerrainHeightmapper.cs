using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainHeightmapper : MonoBehaviour
{
    public bool MAP_TERRAIN;

    public Texture2D heightmap;
    public Terrain terrain;

    private void OnValidate()
    {
        if (MAP_TERRAIN)
        {
            MAP_TERRAIN = false;
            Map();
        }
    }

    void Map()
    {
        int res = terrain.terrainData.heightmapResolution;

        float[,] heights = new float[res, res];

        float conX = heightmap.width / (float)res;
        float conY = heightmap.height / (float)res;

        for (int x = 0; x < res; x++)
            for (int y = 0; y < res; y++)
            {
                heights[y, x] = heightmap.GetPixel((int)(x * conX), (int)(y * conY)).maxColorComponent;
            }
        terrain.terrainData.SetHeights(0, 0, heights);
    }
}
