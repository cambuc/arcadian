using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class TopographicMapper : MonoBehaviour
{
    public bool GENERATE_MAP;
    public string path;
    public bool SAVE_TEXTURE;

    public Terrain terrain;
    public Texture2D texture;
    public Texture2D watermap;
    public int resolution;
    [Range(0, 1)]
    public float cropAmount;

    public float waterThreshold;

    public float steepDistance;
    public float shadowMult;
    public float steepColorMult;

    public int xericAlphaIndex;

    public List<ResourceArea> resources = new List<ResourceArea>();

    [Header("Colors")]
    public Color waterColor;
    public Color temperate;
    public Color xeric;


#if UNITY_EDITOR
    private void OnValidate()
    {
        if (GENERATE_MAP)
        {
            GENERATE_MAP = false;
            GenerateMap();
        }
        if (SAVE_TEXTURE)
        {
            SAVE_TEXTURE = false;

            SaveTexture();
        }
    }
#endif

    void GenerateMap()
    {
        float [,,] alphas = terrain.terrainData.GetAlphamaps(0, 0, terrain.terrainData.alphamapResolution, terrain.terrainData.alphamapResolution);
        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                Vector3 worldPos = TextureToWorldPoint(x, y);
                Vector2Int waterPos = WorldToWaterPos(worldPos);
                if(watermap.GetPixel(waterPos.x, waterPos.y).r >= waterThreshold)
                {
                    texture.SetPixel(x, y, waterColor);
                    continue;
                }

                Vector2Int alphaPos = WorldToAlphaPoint(worldPos);
                Vector2 normPos = WorldToNormalized(worldPos);

                float h = terrain.SampleHeight(new Vector3(worldPos.x, 0, worldPos.z));
                float s = terrain.terrainData.GetSteepness(normPos.x, normPos.y);
                float hCompare = terrain.SampleHeight(new Vector3(worldPos.x + steepDistance, 0, worldPos.z - steepDistance));
                s += shadowMult * (h - hCompare);

                Color steepness = new Color(s * steepColorMult, s * steepColorMult, s * steepColorMult, 0);

                texture.SetPixel(x, y, (alphas[alphaPos.x, alphaPos.y, xericAlphaIndex] > 0.5f ? xeric : temperate) - steepness);
            }
        }

        //Resource Icons
        foreach(ResourceArea res in resources)
        {
            if (!res.mapIcon) continue;

            Vector2Int center = WorldToMapPoint(res.transform.position);
            Vector2Int anchor = new Vector2Int(center.x - res.mapIcon.width / 2, center.y - res.mapIcon.height / 2);
            for(int x = anchor.x; x < anchor.x + res.mapIcon.width; x++)
            {
                for (int y = anchor.y; y < anchor.y + res.mapIcon.height; y++)
                {
                    texture.SetPixel(x, y, res.mapIcon.GetPixel(x - anchor.x, y - anchor.y));
                }
            }

        }

        texture.Apply();
    }

#if UNITY_EDITOR
    void SaveTexture()
    {
        byte[] bytes = texture.EncodeToPNG();

        File.WriteAllBytes(path, bytes);

        AssetDatabase.Refresh();
    }
#endif

    Vector3 TextureToWorldPoint(int x, int y)
    {
        float wx = x * ((terrain.terrainData.size.x * cropAmount) / resolution);
        float wz = y * ((terrain.terrainData.size.z * cropAmount) / resolution);

        float wy = terrain.SampleHeight(new Vector3(wx, 0, wz)) + terrain.transform.position.y;

        return new Vector3(wx, wy, wz);
    }
    Vector2Int WorldToWaterPos(Vector3 worldPos)
    {
        float mx = worldPos.x * ((float)watermap.width / terrain.terrainData.size.x);
        float my = worldPos.z * ((float)watermap.height / terrain.terrainData.size.z);

        return new Vector2Int((int)mx, (int)my);
    }
    Vector2Int WorldToAlphaPoint(Vector3 pos)
    {
        float mx = pos.x * (terrain.terrainData.alphamapResolution / terrain.terrainData.size.x);
        float my = pos.z * (terrain.terrainData.alphamapResolution / terrain.terrainData.size.z);

        return new Vector2Int((int)mx, (int)my);
    }
    Vector2Int WorldToMapPoint(Vector3 pos)
    {
        float mx = pos.x * (resolution / terrain.terrainData.size.x);
        float my = pos.z * (resolution / terrain.terrainData.size.z);

        return new Vector2Int((int)mx, (int)my);
    }
    Vector2 WorldToNormalized(Vector3 pos)
    {
        float mx = pos.x / terrain.terrainData.size.x;
        float my = pos.z / terrain.terrainData.size.z;

        return new Vector2(mx, my);
    }
}
