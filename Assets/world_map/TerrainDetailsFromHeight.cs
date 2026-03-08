using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainDetailsFromHeight : MonoBehaviour
{
    public bool MAP;

    [System.Serializable]
    public struct Layer
    {
        public int index;
        public List<HeightRange> ranges;
    }
    [System.Serializable]
    public struct HeightRange
    {
        public float low;
        public float high;
        public int density;
    }
    public List<Layer> layers = new List<Layer>();

    Terrain terrain;
    TerrainData data;

    private void OnValidate()
    {
        if (MAP)
        {
            MAP = false;

            terrain = GetComponent<Terrain>();
            if (terrain == null)
                return;

            data = terrain.terrainData;

            Map();
        }
    }

    void Map()
    {
        for(int i = 0; i < layers.Count; i++)
        {
            int[,] details = new int[data.detailResolution, data.detailResolution];
            for (int x = 0; x < data.detailResolution; x++)
                for (int y = 0; y < data.detailResolution; y++)
                {
                    float conv = (float)data.heightmapResolution / data.detailResolution;
                    float height = data.GetHeight((int)(y * conv), (int)(x * conv));

                    foreach(HeightRange range in layers[i].ranges)
                    {
                        if (height < range.low || height > range.high)
                            continue;

                        details[x, y] = range.density;
                    }
                }
            data.SetDetailLayer(Vector2Int.zero, i, details);
        }
    }
}
