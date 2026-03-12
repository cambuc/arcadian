using System.Collections.Generic;
using UnityEngine;

public class TemperatureZone : MonoBehaviour
{
    public float radius;
    public bool falloff;
    public float temperatureModifier;

    public bool WithinZone()
    {
        return Physics.OverlapSphere(transform.position, radius, LayerMask.GetMask("Player")).Length > 0;
    }
    public float Distance()
    {
        return Vector3.Distance(transform.position, PlayerMovement.runtime.transform.position);
    }

    public float GetModifier()
    {
        if (!WithinZone())
            return 0;

        if (falloff)
        {
            return Mathf.Lerp(temperatureModifier, 0, Distance() / radius);
        }
        return temperatureModifier;
    }

    private void OnEnable()
    {
        TemperatureManager.runtime.AddZone(this);
    }
    private void OnDestroy()
    {
        TemperatureManager.runtime.RemoveZone(this);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
