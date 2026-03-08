using System.Collections.Generic;
using UnityEngine;

public class PreventWorldFalling : MonoBehaviour
{
    private void Update()
    {
        if (!Physics.Raycast(new Ray(transform.position + Vector3.up, Vector3.down), 100, LayerMask.GetMask("Ground")))
        {
            Physics.Raycast(new Ray(transform.position + 100 * Vector3.up, Vector3.down), out RaycastHit hit, 1000, LayerMask.GetMask("Ground"));
            transform.position = hit.point + Vector3.up;
            if (GetComponent<Rigidbody>()) GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        }
    }
}
