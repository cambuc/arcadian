using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FaunaSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct FaunaType
    {
        public FaunaBehavior fauna;
        public int maintainCount;
        [HideInInspector]
        public List<FaunaBehavior> instances;
        public List<Transform> roamLocations;
        public Vector2Int packAmountRandRange;
    }
    public List<FaunaType> types = new List<FaunaType>();

    private void Start()
    {
        MaintainFaunaCounts();
    }

    void OnDeath(FaunaType type, FaunaBehavior instance)
    {
        type.instances.Remove(instance);
        MaintainFaunaCounts();
    }

    void MaintainFaunaCounts()
    {
        foreach (FaunaType type in types)
        {
            for (int i = type.instances.Count; i < type.maintainCount; i++)
            {
                Transform presetSpawn = type.roamLocations[Random.Range(0, type.roamLocations.Count)];
                FaunaBehavior newFauna = SpawnAnimal(type, presetSpawn);
                for (int f = 1; f < Random.Range(type.packAmountRandRange.x, type.packAmountRandRange.y); f++)
                {
                    FaunaBehavior follower = SpawnAnimal(type, presetSpawn);
                    follower.following = newFauna;
                    newFauna = follower;
                    i++;
                }
            }
        }
    }

    FaunaBehavior SpawnAnimal(FaunaType type, Transform presetSpawn)
    {
        FaunaBehavior inst = Instantiate(type.fauna.gameObject, transform).GetComponent<FaunaBehavior>();

        inst.transform.position = presetSpawn ? presetSpawn.position : 
                                                type.roamLocations[Random.Range(0, type.roamLocations.Count)].position;
        inst.roamLocations = type.roamLocations;

        inst.onDeath.AddListener(() => OnDeath(type, inst));

        type.instances.Add(inst);
        return inst;
    }
}
