using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;

public class TerrainForester : MonoBehaviour
{
    public bool GENERATE_FOREST;
    public bool CLEAR_FOREST;

    public Terrain terrain;

    [System.Serializable]
    public struct Tree
    {
        public List<GameObject> prefabs;
        public float heightOffset;
        public int count;
        public Vector2 scaleRange;
    }
    public List<Tree> trees = new List<Tree>();

    public List<GameObject> temp = new List<GameObject>();

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (GENERATE_FOREST)
        {
            GENERATE_FOREST = false;
            EditorApplication.delayCall += GenerateForest;
        }
        if (CLEAR_FOREST)
        {
            CLEAR_FOREST = false;
            EditorApplication.delayCall += ClearTemp;
        }
    }
#endif

    void GenerateForest()
    {
        ClearTemp();
        foreach (Tree tree in trees)
        {
            for(int i = 0; i < tree.count; i++)
            {
                float x = Random.Range(0, terrain.terrainData.size.x);
                float y = Random.Range(0, terrain.terrainData.size.z);

                GameObject instance = Instantiate(tree.prefabs[Random.Range(0, tree.prefabs.Count)]);
                instance.transform.parent = transform;

                float scale = Random.Range(tree.scaleRange.x, tree.scaleRange.y);
                Vector3 start = instance.transform.localScale;
                instance.transform.localScale = new Vector3(start.x * scale, start.y * scale, start.z * scale);

                Ray ray = new Ray(new Vector3(x, 1000, y), Vector3.down);
                Physics.Raycast(ray, out RaycastHit hit, 2000, LayerMask.GetMask("Ground"));
                if (hit.point == null)
                    continue;

                instance.transform.position = new Vector3(x, hit.point.y + (tree.heightOffset * scale), y);

                instance.transform.eulerAngles = new Vector3(instance.transform.eulerAngles.x,
                    Random.Range(0, 360), instance.transform.eulerAngles.z);

                temp.Add(instance);
            }
        }
    }

    void ClearTemp()
    {
        for (int i = 0; i < temp.Count; i++)
            DestroyImmediate(temp[i]);
        temp.Clear();
    }
}
