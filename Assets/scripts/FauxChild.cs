using System.Collections.Generic;
using UnityEngine;

public class FauxChild : MonoBehaviour
{
    public Transform parent;

    Vector3 posOffset;

    public void SetParent(Transform parent)
    {
        this.parent = parent;

        posOffset = transform.position - parent.transform.position;
    }

    void Update()
    {
        if (!parent)
            return;

        transform.position = parent.transform.position + posOffset;
    }
}
