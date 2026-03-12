using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BulletPhysics : MonoBehaviour
{
    public float damage;
    public float velocity;
    public float gravity;
    public float rayLength;
    Rigidbody rb;

    public float maxLifetime = 100;

    [Header("Debug Options")]
    public bool showLine;
    public GameObject segmentPrefab;

    Vector3 lastPos;
    Vector3 initialPos;

    public void Shoot()
    {
        lastPos = transform.position;
        initialPos = transform.position;

        rb = GetComponent<Rigidbody>();

        rb.AddForce(transform.forward * velocity, ForceMode.Impulse);

        Destroy(gameObject, maxLifetime);
    }

    private void Update()
    {
        rb.AddForce(Vector3.down * gravity, ForceMode.Acceleration);

        Ray ray = new Ray(transform.position, transform.forward);
        Physics.Raycast(ray, out RaycastHit hit, rayLength);

        if (hit.transform && hit.transform.gameObject.layer != LayerMask.GetMask("Equip")) OnHit(hit);

        if (!showLine) return;

        GameObject segment = Instantiate(segmentPrefab, null);
        segment.transform.position = lastPos;
        lastPos = transform.position;
        Destroy(segment, 25);
    }

    void OnHit(RaycastHit hit)
    {
        Debug.Log($"{hit.transform.gameObject.name}, {Vector3.Distance(initialPos, transform.position)} yards");

        if (hit.transform.GetComponent<Attackable>())
            hit.transform.GetComponent<Attackable>().OnHit(damage);

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * rayLength);
    }
}
