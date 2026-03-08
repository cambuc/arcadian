using UnityEngine;
using System.Collections.Generic;

public class TerrainZone : MonoBehaviour
{
    public Terrain terrain;

    public float gizmoHeight = 5;

    public List<Bounds> inclusions;
    public List<Bounds> exclusions;
    public Vector2 heightRange;

    [System.Serializable]
    public struct Bounds
    {
        public Vector2 start;
        public Vector2 end;
    }

    public struct DetailBounds
    {
        public Vector2Int start;
        public Vector2Int end;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        foreach(Bounds b in inclusions)
        {
            DrawBounds(b);
        }
        Gizmos.color = Color.red;
        foreach (Bounds b in exclusions)
        {
            DrawBounds(b);
        }
    }

    void DrawBounds(Bounds b)
    {
        Vector3 center = new Vector3((b.start.x + b.end.x) / 2f, 0, (b.start.y + b.end.y) / 2f);
        Vector3 size = new Vector3(b.end.x - b.start.x, gizmoHeight, b.end.y - b.start.y);

        Gizmos.DrawWireCube(center, size);
    }
}
