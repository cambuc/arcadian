using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainZoner : MonoBehaviour
{
    public bool WRITE_ZONES;

    public Terrain t;

    [System.Serializable]
    public struct Zone
    {
        public int index;
        public Vector2 elevationRange;
        public int groundTextureIndex;
    }
    public List<Zone> zones = new List<Zone>();

    private void OnValidate()
    {
        if (WRITE_ZONES)
        {
            WRITE_ZONES = false;
            WriteZones();
        }
    }

    void WriteZones()
    {
        TerrainData d = t.terrainData;
        int res = d.alphamapResolution;
        float[,,] map = new float[res, res, d.alphamapLayers];

        for(int x = 0; x < res; x++)
            for (int y = 0; y < res; y++)
                for (int i = 0; i < d.alphamapLayers; i++)
                {
                    map[x, y, i] = 0;
                    map[x, y, GetZoneFromAlphamap(x, y).groundTextureIndex] = 1;
                }

        d.SetAlphamaps(0, 0, map);
    }

    public Zone GetZoneFromAlphamap(int x, int y)
    {
        float conv = t.terrainData.heightmapResolution / (float)t.terrainData.alphamapResolution;
        float h = t.terrainData.GetHeight((int)(y * conv), (int)(x * conv));

        foreach(Zone z in zones)
        {
            if(h >= z.elevationRange.x && h < z.elevationRange.y)
            {
                return z;
            }
        }
        return zones[0];
    }
    public Zone GetZoneFromWorld(float x, float y)
    {
        float conv = t.terrainData.heightmapResolution / (float)t.terrainData.size.x;
        float h = t.terrainData.GetHeight((int)(x * conv), (int)(y * conv));

        foreach (Zone z in zones)
        {
            if (h >= z.elevationRange.x && h < z.elevationRange.y)
            {
                return z;
            }
        }
        return zones[0];
    }
    public float GetHeightFromWorld(float x, float y)
    {
        float conv = t.terrainData.heightmapResolution / (float)t.terrainData.size.x;
        return t.terrainData.GetHeight((int)(x * conv), (int)(y * conv));
    }
}
