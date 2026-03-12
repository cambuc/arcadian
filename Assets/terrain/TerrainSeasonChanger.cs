using System.Collections.Generic;
using UnityEngine;

public class TerrainSeasonChanger : MonoBehaviour
{
    public Terrain terrain;

    public TerrainDetailPainter detailPainter;

    public int snowLayerIndex;
    public int snowedOverLayerIndex;

    public Material ponderosaLeaf;
    public Texture2D ponderosaPlain;
    public Texture2D ponderosaSnowy;

    public List<HiddenWinterDetail> hiddenDetails = new List<HiddenWinterDetail>();
    [System.Serializable]
    public struct HiddenWinterDetail
    {
        public int index;
        public float density;
    }

    public Material grassMaterial;
    public Color springGrassColor;
    public Color summerGrassColor;
    public Color fallGrassColor;

    private void Awake()
    {
        TimeManager.seasonChanged.AddListener(OnSeasonChanged);
    }

    public void OnSeasonChanged()
    {
        if(TimeManager.currentSeason == 4)
        {
            if (ponderosaLeaf.mainTexture == ponderosaSnowy)
                return;

            DetailPrototype[] protos = terrain.terrainData.detailPrototypes;
            foreach (HiddenWinterDetail i in hiddenDetails)
            {
                protos[i.index].density = 0;
            }
            terrain.terrainData.detailPrototypes = protos;

            float[,,] map = terrain.terrainData.GetAlphamaps(0, 0, terrain.terrainData.alphamapResolution, terrain.terrainData.alphamapResolution);
            for (int x = 0; x < map.GetLength(0); x++)
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    map[x, y, snowLayerIndex] = map[x, y, snowedOverLayerIndex];
                    map[x, y, snowedOverLayerIndex] = 0;
                }
            terrain.terrainData.SetAlphamaps(0, 0, map);

            ponderosaLeaf.mainTexture = ponderosaSnowy;
        }
        else if (TimeManager.currentSeason == 1)
        {
            grassMaterial.color = springGrassColor;

            if (ponderosaLeaf.mainTexture == ponderosaPlain)
                return;

            DetailPrototype[] protos = terrain.terrainData.detailPrototypes;
            foreach (HiddenWinterDetail i in hiddenDetails)
            {
                protos[i.index].density = i.density;
            }
            terrain.terrainData.detailPrototypes = protos;

            float[,,] map = terrain.terrainData.GetAlphamaps(0, 0, terrain.terrainData.alphamapResolution, terrain.terrainData.alphamapResolution);
            for (int x = 0; x < map.GetLength(0); x++)
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    map[x, y, snowedOverLayerIndex] = map[x, y, snowLayerIndex];
                    map[x, y, snowLayerIndex] = 0;
                }
            terrain.terrainData.SetAlphamaps(0, 0, map);

            ponderosaLeaf.mainTexture = ponderosaPlain;
        }
        else if (TimeManager.currentSeason == 2)
        {
            grassMaterial.color = summerGrassColor;
        }
        else if (TimeManager.currentSeason == 3)
        {
            grassMaterial.color = fallGrassColor;
        }
    }
}
