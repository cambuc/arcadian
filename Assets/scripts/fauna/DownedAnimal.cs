using System.Collections.Generic;
using UnityEngine;

public class DownedAnimal : Interactable
{
    public List<ItemQuantity> harvestItems = new List<ItemQuantity>();
    public AudioClip harvestSound;

    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Interactable");

        interactOptions.Add(new InteractOption() { text = "Harvest", interactTime = 0.5f });
    }

    public override void Interact(string interactOption)
    {
        if (interactOption == "Harvest")
        {
            Harvest();
        }
    }

    void Harvest()
    {
        new UniversalPlayer(harvestSound);
        Fader.runtime.FadeOut(() =>
        {
            foreach (ItemQuantity iq in harvestItems)
                for (int i = 0; i < iq.amount; i++)
                    PlayerInventory.runtime.AddItem(iq.item);

            foreach (FlyingArrow arrow in GetComponentsInChildren<FlyingArrow>())
            {
                arrow.Interact("Store");
            }

            PlayerInteract.runtime.OnExitZone();
            Destroy(gameObject);
            Fader.runtime.FadeIn();
        });
    }
}
