using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTexturePainter : MonoBehaviour
{
    public bool PAINT;

    public Terrain terrain;

    public int flatLayerIndex;
    public int steepLayerIndex;

    public float steepnessThreshold;
    public float perlinZoom = 0.1f;

    TerrainData data;

    private void OnValidate()
    {
        if (PAINT)
        {
            PAINT = false;
            Paint();
        }
    }

    void Paint()
    {
        if (terrain == null)
            return;
        data = terrain.terrainData;

        int res = data.alphamapResolution;
        float[,,] alphas = new float[res, res, data.alphamapLayers];
        for (int x = 0; x < res; x++)
            for (int y = 0; y < res; y++)
            {
                alphas[x, y, flatLayerIndex] = 1;

                float conv = (float)data.heightmapResolution / res;
                float h1 = data.GetHeight((int)(y * conv), (int)(x * conv));
                float h2 = data.GetHeight((int)(y * conv), x + 1 >= res ? (int)(x * conv) - 1 : (int)(x * conv) + 1);

                //float p = Mathf.PerlinNoise(x * perlinZoom, y * perlinZoom);
                float a = Mathf.Abs(h2 - h1) >= steepnessThreshold ? 1 : 0;

                alphas[x, y, flatLayerIndex] = Mathf.Clamp01(1 - a);
                alphas[x, y, steepLayerIndex] = a;
            }
        data.SetAlphamaps(0, 0, alphas);
    }
}
