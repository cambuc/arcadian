using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Rendering;
using static TerrainZone;

public class TerrainDetailPainter : MonoBehaviour
{
    public bool PAINT_DETAILS;

    public Terrain terrain;
    public Texture2D waterMap;

    [System.Serializable]
    public struct DetailLayer
    {
        public string name;

        public int layerIndex;

        public float distanceFromWater;

        public List<ZoneDensity> zones;
    }
    public List<DetailLayer> detailLayers = new List<DetailLayer>();

    [System.Serializable]
    public struct ZoneDensity
    {
        public int density;
        public TerrainZone zone;
    }

    private void OnValidate()
    {
        if (PAINT_DETAILS)
        {
            PAINT_DETAILS = false;
            PaintDetails();
        }
    }

    public void PaintDetails()
    {
        foreach(DetailLayer layer in detailLayers)
        {
            int[,] details = new int[terrain.terrainData.detailResolution, terrain.terrainData.detailResolution];

            foreach (ZoneDensity zone in layer.zones)
                foreach (TerrainZone.Bounds wInclude in zone.zone.inclusions)
                {
                    TerrainZone.DetailBounds include = ToDetailBounds(wInclude);
                    for (int x = include.start.x; x < include.end.x; x++)
                        for (int y = include.start.y; y < include.end.y; y++)
                        {
                            Vector3 worldPos = MapToWorldPoint(x, y);
                            if (worldPos.y < zone.zone.heightRange.x || worldPos.y > zone.zone.heightRange.y ||
                                (layer.distanceFromWater > 0 && IsNearWater(worldPos, layer.distanceFromWater)) || 
                                IsWithinBounds(zone.zone.exclusions, x, y))
                                    continue;

                            details[x, y] = zone.density;
                        }
                }

            terrain.terrainData.SetDetailLayer(0, 0, layer.layerIndex, details);
        }
    }

    Vector3 MapToWorldPoint(int x, int y)
    {
        float z = x * (terrain.terrainData.size.x / terrain.terrainData.detailResolution);
        float wx = y * (terrain.terrainData.size.z / terrain.terrainData.detailResolution);

        return new Vector3(wx, terrain.SampleHeight(new Vector3(wx, 0, z)), z);
    }
    public bool IsNearWater(Vector3 position, float minDistance)
    {
        float d = 1 - waterMap.GetPixel((int)(position.x * (waterMap.width / terrain.terrainData.size.x)),
                                        (int)(position.z * (waterMap.height / terrain.terrainData.size.z))).r;

        return d * 255 < minDistance;
    }

    bool IsWithinBounds(List<TerrainZone.Bounds> bounds, int x, int y)
    {
        foreach(TerrainZone.Bounds wBound in bounds)
        {
            DetailBounds bound = ToDetailBounds(wBound);
            if (x >= bound.start.x && x <= bound.end.x && y >= bound.start.y && y <= bound.end.y)
                return true;
        }
        return false;
    }


    public DetailBounds ToDetailBounds(TerrainZone.Bounds bounds)
    {
        float xMult = terrain.terrainData.detailResolution / terrain.terrainData.size.z;
        float yMult = terrain.terrainData.detailResolution / terrain.terrainData.size.x;

        return new DetailBounds()
        {
            start = new Vector2Int((int)(bounds.start.x * xMult), (int)(bounds.start.y * yMult)),
            end = new Vector2Int((int)(bounds.end.x * xMult), (int)(bounds.end.y * yMult))
        };
    }
}
