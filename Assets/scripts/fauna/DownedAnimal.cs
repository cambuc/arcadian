using System.Collections.Generic;
using UnityEngine;

public class DownedAnimal : Interactable
{
    public List<ItemQuantity> harvestItems = new List<ItemQuantity>();
    public AudioClip harvestSound;

    FaunaBehavior fb;

    public void Initiate(FaunaBehavior fb)
    {
        this.fb = fb;
        gameObject.layer = LayerMask.NameToLayer("Interactable");
        interactable = true;

        harvestItems = fb.harvestItems;
        harvestSound = fb.harvestCarcassSound;
        interactName = $"{fb.faunaName} Carcass";

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
        new UniversalPlayer(harvestSound, MixerGroupHolder.runtime.sfx);
        Fader.runtime.FadeOut(() =>
        {
            if (fb.skinSack)
            {
                Container inst = Instantiate(fb.skinSack.gameObject).GetComponent<Container>();
                inst.transform.position = transform.position;
                foreach (ItemQuantity iq in harvestItems)
                    for (int i = 0; i < iq.amount; i++)
                        inst.items.Add(iq.item);
                inst.SetInteractOptions();
            }
            else
            {
                foreach (ItemQuantity iq in harvestItems)
                    for (int i = 0; i < iq.amount; i++)
                        PlayerInventory.runtime.AddItem(iq.item);
            }

            foreach (FlyingArrow arrow in GetComponentsInChildren<FlyingArrow>())
            {
                arrow.Interact("Pick Up");
            }

            PlayerInteract.runtime.OnExitZone();
            Fader.runtime.FadeIn();
            Destroy(gameObject);
        });
    }
}
