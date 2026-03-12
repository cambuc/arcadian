using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;

    [Header("Ray Segment Settings")]
    public float segmentLength;
    public int segmentCount;
    public float gravityAngle;

    public void Shoot()
    {
        Vector3 origin = transform.position;
        for(int i = 0; i < segmentCount; i++)
        {
            Ray ray = new Ray(origin, transform.forward);
            Physics.Raycast(ray, out RaycastHit hit, segmentLength);

            if (hit.transform)
            {
                OnHit(hit);
                return;
            }

            origin += transform.forward * segmentLength;
            transform.Rotate(Vector3.right, gravityAngle);
        }
        Destroy(gameObject);
    }

    void OnHit(RaycastHit hit)
    {
        Attackable attackable = hit.transform.GetComponent<Attackable>();
        if (!attackable)
        {
            Transform parent = hit.transform.parent;
            while (parent != null)
            {
                if (parent.GetComponent<Attackable>())
                {
                    attackable = parent.GetComponent<Attackable>();
                    break;
                }
                parent = parent.parent;
            }
        }

        if (attackable)
            attackable.OnHit(damage);

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;
        for (int i = 0; i < segmentCount; i++)
        {
            Gizmos.color = Color.Lerp(Color.green, Color.red, (float)i / segmentCount);

            Gizmos.DrawLine(origin, origin + direction * segmentLength);

            origin += direction * segmentLength;
            direction = Vector3.RotateTowards(direction, -Vector3.up, (i + 1) * gravityAngle * (Mathf.PI / 180f), float.MaxValue);
        }
    }
}
