using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AdaptivePerformance.Provider;

public class TerrainPlacementTool : MonoBehaviour
{
    public bool GENERATE_FEATURES;
    public bool CLEAR_FEATURES;

    public Terrain terrain;
    public Texture2D waterMap;
    public Vector2 heightRange;

    [System.Serializable]
    public struct TerrainFeature
    {
        public string name;
        public List<GameObject> variants;
        public float heightOffset;
        public int count;
        public Vector2 scaleRange;
        public Vector3 rotationAxes;
        public float distanceFromWater;
        public bool insideExcludes;
    }
    public List<TerrainFeature> terrainFeatures = new List<TerrainFeature>();

    [HideInInspector]
    public List<ExclusionZone> excludes = new List<ExclusionZone>();

    public List<GameObject> temp = new List<GameObject>();
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (GENERATE_FEATURES)
        {
            GENERATE_FEATURES = false;
            EditorApplication.delayCall += GenerateFeatures;
        }
        if (CLEAR_FEATURES)
        {
            CLEAR_FEATURES = false;
            EditorApplication.delayCall += ClearTemp;
        }
    }
#endif

    void GenerateFeatures()
    {
        ClearTemp();
        foreach (TerrainFeature tf in terrainFeatures)
        {
            GameObject parent = new GameObject(tf.name);
            parent.transform.parent = transform;
            temp.Add(parent);
            for (int i = 0; i < tf.count; i++)
            {
                float x = Random.Range(0, terrain.terrainData.size.x);
                float y = Random.Range(0, terrain.terrainData.size.z);

                bool skip = tf.insideExcludes;
                foreach (ExclusionZone exclude in FindObjectsByType<ExclusionZone>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
                {
                    if (exclude.tpt == this && exclude.WithinBounds(x, y))
                        skip = !tf.insideExcludes;
                }
                if (skip)
                    continue;

                Ray ray = new Ray(new Vector3(x, 1000, y), Vector3.down);
                Physics.Raycast(ray, out RaycastHit hit, 2000, LayerMask.GetMask("Ground"));
                if (hit.point == null || hit.point.y < heightRange.x || hit.point.y > heightRange.y || 
                    (tf.distanceFromWater > 0 && IsNearWater(new Vector3(x, hit.point.y, y), tf.distanceFromWater)))
                    continue;

                GameObject instance = Instantiate(tf.variants[Random.Range(0, tf.variants.Count)]);
                instance.transform.parent = parent.transform;

                float scale = Random.Range(tf.scaleRange.x, tf.scaleRange.y);
                Vector3 start = instance.transform.localScale;
                instance.transform.localScale = new Vector3(start.x * scale, start.y * scale, start.z * scale);

                instance.transform.position = new Vector3(x, hit.point.y + (tf.heightOffset * scale), y);

                float eulX = instance.transform.eulerAngles.x + tf.rotationAxes.x * Random.Range(-360, 360);
                float eulY = instance.transform.eulerAngles.y + tf.rotationAxes.y * Random.Range(-360, 360);
                float eulZ = instance.transform.eulerAngles.z + tf.rotationAxes.z * Random.Range(-360, 360);
                instance.transform.eulerAngles = new Vector3(eulX, eulY, eulZ);

                if (instance.GetComponent<Grounder>() && instance.GetComponent<Grounder>().groundChildrenOnStart)
                    instance.GetComponent<Grounder>().GroundChildren();

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
    public bool IsNearWater(Vector3 position, float minDistance)
    {
        float d = 1 - waterMap.GetPixel((int)(position.x * (waterMap.width / terrain.terrainData.size.x)),
                                        (int)(position.z * (waterMap.height / terrain.terrainData.size.z))).r;

        return d * 255 < minDistance;
    }
}
