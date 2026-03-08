using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static TerrainZoner;

public class TerrainTreePainter : MonoBehaviour
{
    public bool PAINT_TREES;

    public Terrain terrain;
    public Texture2D waterMap;

    [System.Serializable]
    public struct TreeType
    {
        public string name;

        public List<int> protoIndices;

        public Vector2 scaleRange;

        public float distanceFromWater;

        public List<ZoneCount> zones;
    }
    public List<TreeType> treeTypes = new List<TreeType>();

    [System.Serializable]
    public struct ZoneCount
    {
        public int count;
        public TerrainZone zone;
    }

    private void OnValidate()
    {
        if (PAINT_TREES)
        {
            PAINT_TREES = false;
            PaintTrees();
        }
    }

    public void PaintTrees()
    {
        List<TreeInstance> instances = new List<TreeInstance>();
        foreach (TreeType type in treeTypes)
        {
            foreach (ZoneCount zone in type.zones)
                foreach (TerrainZone.Bounds include in zone.zone.inclusions)
                {
                    for (int i = 0; i < zone.count; i++)
                    {
                        Vector3 pos = ToWorldPoint( Random.Range(include.start.x, include.end.x), 
                                                    Random.Range(include.start.y, include.end.y));

                        if (pos.y < zone.zone.heightRange.x || pos.y > zone.zone.heightRange.y ||
                            (type.distanceFromWater > 0 && IsNearWater(pos, type.distanceFromWater)) ||
                            IsWithinBounds(zone.zone.exclusions, pos.x, pos.z))
                                continue;

                        float scale = Random.Range(type.scaleRange.x, type.scaleRange.y);
                        TreeInstance inst = new TreeInstance()
                        {
                            prototypeIndex = type.protoIndices[Random.Range(0, type.protoIndices.Count)],
                            color = Color.white,
                            position = new Vector3( pos.x / terrain.terrainData.size.x,
                                                    pos.y / terrain.terrainData.size.y,
                                                    pos.z / terrain.terrainData.size.z),
                            heightScale = scale,
                            widthScale = scale,
                            rotation = Random.Range(0f, 360f)
                        };
                        instances.Add(inst);
                    }
                }
        }
        terrain.terrainData.SetTreeInstances(instances.ToArray(), true);
    }

    Vector3 ToWorldPoint(float x, float y)
    {
        return new Vector3(x, terrain.SampleHeight(new Vector3(x, 0, y)), y);
    }
    public bool IsNearWater(Vector3 position, float minDistance)
    {
        float d = 1 - waterMap.GetPixel((int)(position.x * (waterMap.width / terrain.terrainData.size.x)),
                                        (int)(position.z * (waterMap.height / terrain.terrainData.size.z))).r;

        return d * 255 < minDistance;
    }

    bool IsWithinBounds(List<TerrainZone.Bounds> bounds, float x, float y)
    {
        foreach (TerrainZone.Bounds bound in bounds)
        {
            if (x >= bound.start.x && x <= bound.end.x && y >= bound.start.y && y <= bound.end.y)
                return true;
        }
        return false;
    }
}
