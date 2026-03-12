using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ResourceArea : MonoBehaviour
{
    public bool GENERATE_RESOURCES;
    public bool CLEAR_RESOURCES;

    public float radius;
    [Range(0, 1)]
    public float density;
    [ReadOnly]
    public int currentCount;

    [Header("Map Properties")]
    public Texture2D mapIcon;
    public string mapName;

    [Header("Object Properties")]
    public List<GameObject> resourceVariants = new List<GameObject>();
    public float heightOffset;
    public Vector3 rotationAxes;
    public Vector2 randomScaleRange;

    public List<GameObject> temp = new List<GameObject>();
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (GENERATE_RESOURCES)
        {
            GENERATE_RESOURCES = false;
            EditorApplication.delayCall += GenerateResources;
        }
        if (CLEAR_RESOURCES)
        {
            CLEAR_RESOURCES = false;
            EditorApplication.delayCall += ClearTemp;
        }
    }
#endif

    void GenerateResources()
    {
        ClearTemp();

        for (float x = transform.position.x - radius; x <= transform.position.x + radius; x++)
        {
            for (float z = transform.position.z - radius; z <= transform.position.z + radius; z++)
            {
                float dx = x - transform.position.x;
                float dz = z - transform.position.z;

                if (dx * dx + dz * dz <= radius * radius && Random.Range(0f, 1f) <= density)
                {
                    GameObject inst = Instantiate(resourceVariants[Random.Range(0, resourceVariants.Count)], transform);

                    float scale = Random.Range(randomScaleRange.x, randomScaleRange.y);
                    Vector3 start = inst.transform.localScale;
                    inst.transform.localScale = new Vector3(start.x * scale, start.y * scale, start.z * scale);

                    inst.transform.position = new Vector3(x, Terrain.activeTerrain.SampleHeight(new Vector3(x, 0, z)) + heightOffset, z);

                    float eulX = inst.transform.eulerAngles.x + rotationAxes.x * Random.Range(-360, 360);
                    float eulY = inst.transform.eulerAngles.y + rotationAxes.y * Random.Range(-360, 360);
                    float eulZ = inst.transform.eulerAngles.z + rotationAxes.z * Random.Range(-360, 360);
                    inst.transform.eulerAngles = new Vector3(eulX, eulY, eulZ);

                    temp.Add(inst);
                }
            }
        }
        currentCount = temp.Count;
    }

    void ClearTemp()
    {
        for (int i = 0; i < temp.Count; i++)
            DestroyImmediate(temp[i]);
        temp.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
