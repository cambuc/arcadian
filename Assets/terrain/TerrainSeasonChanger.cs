using System.Collections.Generic;
using UnityEngine;

public class TerrainSeasonChanger : MonoBehaviour
{
    public Terrain terrain;

    public TerrainDetailPainter detailPainter;

    public int snowLayerIndex;
    public int snowedOverLayerIndex;

    [System.Serializable]
    public struct MaterialChange
    {
        public Material material;
        public Texture2D winter;
        public Texture2D spring;
        public Texture2D summer;
        public Texture2D fall;
    }
    public List<MaterialChange> materialChanges = new List<MaterialChange>();

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

    int season;

    private void Awake()
    {
        TimeManager.seasonChanged.AddListener(OnSeasonChanged);
    }

    public void OnSeasonChanged()
    {
        if(TimeManager.currentSeason == 4 && season != TimeManager.currentSeason)
        {
            season = TimeManager.currentSeason;

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

            for (int i = 0; i < materialChanges.Count; i++)
                materialChanges[i].material.mainTexture = materialChanges[i].winter;
        }
        else if (TimeManager.currentSeason == 1 && season == 4)
        {
            grassMaterial.color = springGrassColor;

            season = TimeManager.currentSeason;

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

            for (int i = 0; i < materialChanges.Count; i++)
                materialChanges[i].material.mainTexture = materialChanges[i].spring;
        }
        else if (TimeManager.currentSeason == 2 && season != TimeManager.currentSeason)
        {
            season = TimeManager.currentSeason;

            grassMaterial.color = summerGrassColor;

            for (int i = 0; i < materialChanges.Count; i++)
                materialChanges[i].material.mainTexture = materialChanges[i].summer;
        }
        else if (TimeManager.currentSeason == 3 && season != TimeManager.currentSeason)
        {
            season = TimeManager.currentSeason;

            grassMaterial.color = fallGrassColor;

            for (int i = 0; i < materialChanges.Count; i++)
                materialChanges[i].material.mainTexture = materialChanges[i].fall;
        }
    }
}
