using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AdaptivePerformance.Provider;
using UnityEngine.UIElements;

public class TerrainWaterMapper : MonoBehaviour
{
    public bool MAP_WATER;
    public string path;
    public bool SAVE_TEXTURE;

    public Terrain terrain;

    public int textureResolution;
    public float maxDistance;

    public Texture2D distanceTexture;

    List<Vector2Int> waterPixels = new List<Vector2Int>();

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (MAP_WATER)
        {
            MAP_WATER = false;

            MapWater();
        }
        if (SAVE_TEXTURE)
        {
            SAVE_TEXTURE = false;

            SaveTexture();
        }
    }
#endif

    void MapWater()
    {
        TerrainData data = terrain.terrainData;

        waterPixels.Clear();
        for (int x = 0; x < textureResolution; x++)
            for (int y = 0; y < textureResolution; y++)
            {
                bool isWater = IsExposedWater(MapToWorldPoint(x, y));
                distanceTexture.SetPixel(x, y, new Color(isWater ? 1 : 0, 0, 0, 1));
                if(isWater)
                    waterPixels.Add(new Vector2Int(x, y));
            }

        for (int x = 0; x < textureResolution; x++)
            for (int y = 0; y < textureResolution; y++)
            {
                distanceTexture.SetPixel(x, y, new Color(1 - (FindNearestWater(x, y) / maxDistance), 0, 0, 1));
            }

        distanceTexture.Apply();
    }

#if UNITY_EDITOR
    void SaveTexture()
    {
        byte[] bytes = distanceTexture.EncodeToPNG();

        File.WriteAllBytes(path, bytes);

        AssetDatabase.Refresh();
    }
#endif
    bool IsExposedWater(Vector3 position)
    {
        float terrainHeight = terrain.SampleHeight(position) + terrain.transform.position.y;

        Ray ray = new Ray(new Vector3(position.x, terrainHeight + 100f, position.z), Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, 200f, LayerMask.GetMask("Water")))
        {
            if (hit.point.y > terrainHeight)
                return true;
        }
        return false;
    }

    float FindNearestWater(int x, int y)
    {
        float distance = textureResolution * textureResolution;
        foreach(Vector2Int pixel in waterPixels)
        {
            if (Vector2.Distance(new Vector2(x, y), pixel) < distance)
                distance = Vector2.Distance(new Vector2(x, y), pixel);
        }
        return distance;
    }

    Vector3 MapToWorldPoint(int x, int y)
    {
        float wx = x * (terrain.terrainData.size.x / textureResolution);
        float wz = y * (terrain.terrainData.size.z / textureResolution);

        float wy = terrain.SampleHeight(new Vector3(wx, 0, wz)) + terrain.transform.position.y;

        return new Vector3(wx, wy, wz);
    }
}