using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grounder : MonoBehaviour
{
    public bool GROUND_OBJECTS;
    public bool GROUND_CHILDREN;

    public bool groundListOnStart;
    public bool groundChildrenOnStart;

    public Terrain terrain;

    public float heightOffset;

    public List<Transform> groundObjects = new List<Transform>();

    private void OnValidate()
    {
        if (GROUND_OBJECTS)
        {
            GROUND_OBJECTS = false;
            Ground(groundObjects);
        }
        if (GROUND_CHILDREN)
        {
            GROUND_CHILDREN = false;
            GroundChildren();
        }
    }

    private void Start()
    {
        if (groundChildrenOnStart)
        {
            GroundChildren();
        }
        if (groundListOnStart)
        {
            Ground(groundObjects);
        }
    }

    public void GroundChildren()
    {
        List<Transform> directChildren = new List<Transform>();
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            if (child.parent == transform) directChildren.Add(child);
        }
        Ground(directChildren);
    }

    public void Ground(List<Transform> objects)
    {
        foreach(Transform obj in objects)
        {
            Ray ray = new Ray(new Vector3(obj.position.x, 1000, obj.position.z), Vector3.down);
            Physics.Raycast(ray, out RaycastHit hit, 2000, LayerMask.GetMask("Ground"));
            if (hit.point != null)
                obj.position = new Vector3(obj.position.x, hit.point.y + (heightOffset * obj.localScale.y), obj.position.z);
        }
    }
}
