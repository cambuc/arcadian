using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TerrainTreeSwapper : MonoBehaviour
{
    public Terrain terrain;

    public float swapDistance;
    [Range(0, 1)]
    public float gridSearchSize;

    [System.Serializable]
    public struct Chunk
    {
        public List<TreeInstance> trees;
        public Vector3 center;

        public Vector2 start;
        public Vector2 end;

        public bool WithinBounds(float x, float y)
        {
            return x >= start.x && x < end.x && y >= start.y && y < end.y;
        }

        public List<GameObject> colliders;

        public void Enable()
        {
            foreach (GameObject col in colliders)
                if(col) col.SetActive(true);
        }
        public void Disable()
        {
            foreach (GameObject col in colliders)
                if (col) col.SetActive(false);
        }
    }
    List<Chunk> chunks = new List<Chunk>();

    [System.Serializable]
    public struct TreeSwap
    {
        public int index;
        public GameObject prefab;
    }
    public List<TreeSwap> treeSwaps = new List<TreeSwap>();

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
        for (float x = 0; x < 1; x += gridSearchSize)
            for (float y = 0; y < 1; y += gridSearchSize)
            {
                Vector3 preCenter = new Vector3(x + gridSearchSize / 2f, 0, y + gridSearchSize / 2f);
                chunks.Add(new Chunk()
                {
                    start = new Vector2(x, y),
                    end = new Vector2(x + gridSearchSize, y + gridSearchSize),
                    center = ToWorldPos(new Vector3(
                        preCenter.x, 
                        terrain.SampleHeight(preCenter) / terrain.terrainData.size.y, 
                        preCenter.z)),
                    trees = new List<TreeInstance>(),
                    colliders = new List<GameObject>()
                });
            }

        foreach (TreeInstance inst in terrain.terrainData.treeInstances)
            foreach (Chunk chunk in chunks)
            {
                if (chunk.WithinBounds(inst.position.x, inst.position.z))
                {
                    chunk.trees.Add(inst);

                    TreeSwap ts = treeSwaps.Where(t => t.index == inst.prototypeIndex).FirstOrDefault();
                    if (ts.prefab == null) continue;

                    GameObject model = Instantiate(ts.prefab, transform);
                    model.transform.position = ToWorldPos(inst.position);

                    //Tree Feller
                    if (model.transform.GetComponent<TreeFeller>())
                        model.transform.GetComponent<TreeFeller>().treeInstance = inst;
                    //Tree Harvest
                    if (model.transform.GetComponent<HarvestTerrainTree>())
                        model.transform.GetComponent<HarvestTerrainTree>().treeInstance = inst;

                    chunk.colliders.Add(model);
                }
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

    Vector3 ToWorldPos(Vector3 treePos)
    {
        return new Vector3( treePos.x * terrain.terrainData.size.x, 
                            treePos.y * terrain.terrainData.size.y, 
                            treePos.z * terrain.terrainData.size.z);
    }
}
