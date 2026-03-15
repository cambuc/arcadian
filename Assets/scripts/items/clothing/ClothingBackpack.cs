using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothingBackpack : Clothing
{
    public List<Item> items = new List<Item>();

    public override WorldItem Drop()
    {
        GameObject instance = Instantiate(worldObject.gameObject);
        instance.GetComponent<Backpack>().items = items;
        instance.transform.position = PlayerMovement.runtime.transform.position + Camera.main.transform.forward + Vector3.up;
        Destroy(gameObject);

        return worldObject.GetComponent<WorldItem>();
    }
}
