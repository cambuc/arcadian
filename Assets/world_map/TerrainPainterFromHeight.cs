using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class TerrainPainterFromHeight : MonoBehaviour
{
    public bool MAP;
    public Terrain terrain;
    public int baseLayer;

    [System.Serializable]
    public struct Layer
    {
        public int index;
        public List<Range> ranges;
    }
    [System.Serializable]
    public struct Range
    {
        public float low;
        public float high;
        public float perlinZoom;
    }
    public List<Layer> layers = new List<Layer>();

    TerrainData data;

    private void OnValidate()
    {
        if (MAP)
        {
            MAP = false;

            Map();
        }
    }

    public void Map()
    {
        if (terrain == null)
            return;
        data = terrain.terrainData;

        int res = data.alphamapResolution;
        float[,,] alphas = new float[res, res, data.alphamapLayers];
        for (int i = 0; i < layers.Count; i++)
        {
            for (int x = 0; x < res; x++)
                for (int y = 0; y < res; y++)
                {
                    alphas[x, y, baseLayer] = 1;

                    float conv = (float)data.heightmapResolution / res;
                    float height = data.GetHeight((int)(y * conv), (int)(x * conv));

                    foreach (Range range in layers[i].ranges)
                    {
                        if (height < range.low || height > range.high)
                        {
                            continue;
                        }

                        float a = Mathf.PerlinNoise(x * range.perlinZoom, y * range.perlinZoom);

                        alphas[x, y, baseLayer] = Mathf.Clamp01(1 - a);
                        alphas[x, y, layers[i].index] = a;
                    }
                }
        }
        data.SetAlphamaps(0, 0, alphas);
    }
}
