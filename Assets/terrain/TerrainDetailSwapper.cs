using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TerrainDetailSwapper : MonoBehaviour
{
    public Terrain terrain;

    public float swapDistance;

    public int detailIndex;

    public GameObject prefab;

    [System.Serializable]
    public struct Chunk
    {
        public Vector3 center;

        public Vector2Int start;
        public Vector2Int end;

        public bool WithinBounds(float x, float y)
        {
            return x >= start.x && x < end.x && y >= start.y && y < end.y;
        }

        public List<GameObject> colliders;

        public void Enable()
        {
            foreach (GameObject col in colliders)
                if (col) col.SetActive(true);
        }
        public void Disable()
        {
            foreach (GameObject col in colliders)
                if (col) col.SetActive(false);
        }
    }
    List<Chunk> chunks = new List<Chunk>();

    private void Awake()
    {
        GameTick.tick.AddListener(OnTick);
    }

    private void Start()
    {
#if UNITY_EDITOR
        GenerateColliders();
#endif
    }

    public void GenerateColliders()
    {
        int[,] layer = terrain.terrainData.GetDetailLayer(0, 0, terrain.terrainData.detailResolution, terrain.terrainData.detailResolution, detailIndex);
        for (int x = 0; x < layer.GetLength(0); x++)
            for (int y = 0; y < layer.GetLength(1); y++)
            {
                if (layer[x, y] <= 0) continue;

                Vector3 preCenter = new Vector3(x + 0.5f, 0, y + 0.5f);
                Chunk chunk = new Chunk()
                {
                    start = new Vector2Int(x, y),
                    end = new Vector2Int(x + 1, y + 1),
                    center = ToWorldPos(preCenter.x, preCenter.z),
                    colliders = new List<GameObject>()
                };
                chunks.Add(chunk);

                GameObject model = Instantiate(prefab, transform);
                model.transform.position = ToWorldPos(x, y);

                chunk.colliders.Add(model);
            }
    }

    void OnTick()
    {
        foreach (Chunk chunk in chunks)
        {
            if (Vector3.Distance(chunk.center, PlayerMovement.runtime.transform.position) <= swapDistance)
            {
                chunk.Enable();
            }
            else
            {
                chunk.Disable();
            }
        }
    }

    Vector3 ToWorldPos(float x, float y)
    {
        Vector3 pos =  new Vector3( y * (terrain.terrainData.size.x / terrain.terrainData.detailResolution),
                                    0,
                                    x * (terrain.terrainData.size.x / terrain.terrainData.detailResolution));

        return new Vector3(pos.x, terrain.SampleHeight(pos), pos.z);
    }
}
