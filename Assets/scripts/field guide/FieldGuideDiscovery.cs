using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FieldGuideDiscovery : MonoBehaviour
{
    public static UnityEvent<FieldGuideSpecies> onDiscover = new UnityEvent<FieldGuideSpecies>();

    public FieldGuideSpecies species;
    public Interactable interact;

    public string unknownName;
    public string knownName;

    private void Awake()
    {
        interact.onInteract.AddListener(Discover);

        onDiscover.AddListener(OnDiscover);
    }

    private void OnValidate()
    {
        if (unknownName == "" && interact)
            unknownName = interact.interactName;
        if (knownName == "" && interact)
            knownName = interact.interactName;
    }

    public void Discover()
    {
        if (species.discovered) return;

        HUDMessage.runtime.ShowMessage($"You've discovered {species.commonName}\nSee the field guide for more details");
        species.discovered = true;

        onDiscover.Invoke(species);
    }

    void OnDiscover(FieldGuideSpecies sp)
    {
        if (sp == species) interact.interactName = knownName;
    }
}
