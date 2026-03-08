using System.Collections.Generic;
using UnityEngine;

public class ThrownStick : WorldItem
{
    public float damage { get; set; }
    public Rigidbody rb;
    public float force;

    public float spinSpeed = 20f;
    public Vector3 spinAxis;
    public float directionRandomness;

    public AudioSource hitSound;

    bool collided;

    private void OnValidate()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
    }

    public void Shoot()
    {
        Vector3 direction = new Vector3(
            Camera.main.transform.forward.x + Random.Range(-directionRandomness, directionRandomness),
            Camera.main.transform.forward.y + Random.Range(-directionRandomness, directionRandomness),
            Camera.main.transform.forward.z + Random.Range(-directionRandomness, directionRandomness)
            );
        rb.AddForce(force * direction, ForceMode.Impulse);
    }

    void FixedUpdate()
    {
        if (collided)
            return;

        rb.angularVelocity = spinAxis * spinSpeed;
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collided || collision.gameObject == PlayerMovement.runtime.gameObject)
            return;

        hitSound.Play();

        Attackable attackable = collision.transform.GetComponent<Attackable>();
        if (!attackable)
        {
            Transform parent = collision.transform.parent;
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
        if (attackable) attackable.OnHit(damage);

        collided = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (collided || other.gameObject == PlayerMovement.runtime.gameObject || other.transform.parent == PlayerMovement.runtime.gameObject)
            return;

        hitSound.Play();

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
        {
            attackable.OnHit(damage);
        }

        collided = true;
    }
}
