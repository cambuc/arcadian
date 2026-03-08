using System.Collections.Generic;
using UnityEngine;

public class FloraDropItems : MonoBehaviour
{
    public List<DropItem> dropItems = new List<DropItem>();
    public Transform dropPoint;
    public float dropRadius;

    private void Start()
    {
        foreach(DropItem dropItem in dropItems)
        {
            TimeManager.runtime.WaitToCall(
                new WaitCall() { action = () => { Drop(dropItem); }, secondsLeft = Random.Range(dropItem.dropTimerRange.x, dropItem.dropTimerRange.y) });
        }
    }

    void Drop(DropItem dropItem)
    {
        GameObject instance = Instantiate(dropItem.worldObject);
        instance.transform.position = dropPoint.position + new Vector3(Random.Range(-dropRadius, dropRadius), 0, Random.Range(-dropRadius, dropRadius));
        TimeManager.runtime.WaitToCall(
            new WaitCall() { action = () => { Drop(dropItem); }, secondsLeft = Random.Range(dropItem.dropTimerRange.x, dropItem.dropTimerRange.y) });
    }

    private void OnDrawGizmosSelected()
    {
        if(dropPoint)
            Gizmos.DrawWireSphere(dropPoint.position, dropRadius);
    }
}

[System.Serializable]
public struct DropItem
{
    public GameObject worldObject;
    public Vector2Int dropTimerRange;
}