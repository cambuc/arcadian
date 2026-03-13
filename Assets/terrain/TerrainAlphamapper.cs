using System.Collections.Generic;
using UnityEngine;

public class TerrainAlphamapper : MonoBehaviour
{
    public bool PAINT_ALPHAS;

    public Terrain terrain;
    public Texture2D biomeMap;
    public Texture2D waterMap;

    public int riparianIndex;
    public float distanceFromWater;

    [System.Serializable]
    public struct Biome
    {
        public Color mapColor;
        public int layerIndex;
    }
    public List<Biome> biomes = new List<Biome>();

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (PAINT_ALPHAS)
        {
            PAINT_ALPHAS = false;
            Paint();
        }
    }
#endif

    void Paint()
    {
        float[,,] alphas = new float[terrain.terrainData.alphamapResolution, terrain.terrainData.alphamapResolution, terrain.terrainData.alphamapLayers];
        for(int x = 0; x < terrain.terrainData.alphamapResolution; x++)
        {
            for (int y = 0; y < terrain.terrainData.alphamapResolution; y++)
            {
                float riparian = 0;
                if(1f - WaterMap(x, y).r <= distanceFromWater)
                {
                    riparian = (1f / distanceFromWater) * (WaterMap(x, y).r - (1 - distanceFromWater));
                    alphas[x, y, riparianIndex] = riparian;
                }
                if(riparian < 1)
                {
                    bool found = false;
                    foreach(Biome b in biomes)
                    {
                        if(b.mapColor == BiomeMap(x, y))
                        {
                            alphas[x, y, b.layerIndex] = 1f - riparian;
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        foreach (Biome b in biomes)
                            alphas[x, y, b.layerIndex] = (1f - riparian) / biomes.Count;
                    }
                }
            }
        }
        terrain.terrainData.SetAlphamaps(0, 0, alphas);
    }

    Color WaterMap(int x, int y)
    {
        float mx = x * ((float)waterMap.width / terrain.terrainData.alphamapResolution);
        float my = y * ((float)waterMap.height / terrain.terrainData.alphamapResolution);

        return waterMap.GetPixel((int)my, (int)mx);
    }
    Color BiomeMap(int x, int y)
    {
        float mx = x * ((float)biomeMap.width / terrain.terrainData.alphamapResolution);
        float my = y * ((float)biomeMap.height / terrain.terrainData.alphamapResolution);

        return biomeMap.GetPixel((int)mx, (int)my);
    }
}
