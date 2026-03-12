using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FlyingArrowHitbox : MonoBehaviour
{
    public FlyingArrow flyingArrow;
    public float collisionRadius;

    void Update()
    {
        Collider[] collisions = Physics.OverlapSphere(transform.position, collisionRadius);
        if (collisions.Length <= 0)
            return;

        Collider collision = collisions[0];
        foreach (Collider col in collisions)
            if (col.gameObject != flyingArrow.gameObject && col.gameObject != PlayerMovement.runtime.gameObject)
            {
                collision = col;
                break;
            }

        //flyingArrow.OnHit(collision.gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, collisionRadius);
    }
}
