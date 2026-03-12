using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TreeFeller : MonoBehaviour
{
    public TreeInstance treeInstance;
    public GameObject felledVersion;
    public int timeInSeconds;

    public Transform alignmentObject;

    public List<Item> fellingTools = new List<Item>();

    public void Interact()
    {
        PlayerInventoryUI.runtime.OpenFilteredInventory(fellingTools, FellTree);
    }

    void FellTree(Item implement)
    {
        if (implement == null)
            return;

        Tool tool = (Tool)implement;
        new UniversalPlayer(tool.useSound, MixerGroupHolder.runtime.sfx);
        tool.LoseCondition();

        Fader.runtime.FadeOut(() =>
        {
            GameObject felled = Instantiate(felledVersion);
            felled.transform.position = alignmentObject.position;
            felled.transform.eulerAngles = alignmentObject.eulerAngles + new Vector3(1, 0, 1);
            felled.transform.localScale = alignmentObject.lossyScale;

            TimeManager.runtime.PassTime(timeInSeconds * implement.GetComponent<Tool>().timeMultiplier);
            Fader.runtime.FadeIn();

            List<TreeInstance> treeTemp = Terrain.activeTerrain.terrainData.treeInstances.ToList();
            treeTemp.Remove(treeInstance);
            Terrain.activeTerrain.terrainData.treeInstances = treeTemp.ToArray();
            Destroy(gameObject);
        });
    }
}
