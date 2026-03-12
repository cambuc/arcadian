using System.Collections.Generic;
using UnityEngine;

public class FlyingArrow : WorldItem
{
    public float damage { get; set; }
    public Rigidbody rb;
    public Collider col;
    public float force;
    public float hitEmbedDistance;
    public float arrowHeadWeighDown;

    public SoundPlayer hitSound;

    private void OnValidate()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
    }

    public void Shoot()
    {
        rb.AddForce(force * Camera.main.transform.forward, ForceMode.Impulse);
    }

    void FixedUpdate()
    {
        if (rb.isKinematic)
            return;

        Vector3 arrowForward = transform.forward;
        Vector3 downwardDirection = Vector3.down;
        Quaternion targetRotation = Quaternion.LookRotation(downwardDirection, transform.up);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * arrowHeadWeighDown));
    }
    bool collided;
    private void OnTriggerEnter(Collider other)
    {
        if (collided || other.gameObject == PlayerMovement.runtime.gameObject || other.transform.parent == PlayerMovement.runtime.gameObject)
            return;
        collided = true;
        hitSound.PlaySound();

        rb.isKinematic = true;
        col.isTrigger = true;
        transform.parent = other.transform;
        transform.position += hitEmbedDistance * transform.forward;

        Attackable attackable = other.transform.GetComponent<Attackable>();
        if (!attackable)
        {
            Transform parent = other.transform.parent;
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
    }
}
