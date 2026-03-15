using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WaterSource : Interactable
{
    public float drinkTime = 1;
    public float searchTime = 1;

    public Item stoneItem;

    public SoundPlayer waterPlayer;
    public SoundPlayer stonePlayer;

    public Material frozenMaterial;
    public Material plainMaterial;
    public bool freezeInWinter;


    private void Awake()
    {
        if(freezeInWinter)
            TimeManager.seasonChanged.AddListener(OnSeasonChange);
    }

    void OnSeasonChange()
    {
        if(TimeManager.currentSeason == 4)
        {
            GetComponent<Collider>().isTrigger = false;
            interactable = false;
            GetComponent<Renderer>().material = frozenMaterial;
        }
        else
        {
            GetComponent<Collider>().isTrigger = true;
            interactable = true;
            GetComponent<Renderer>().material = plainMaterial;
        }
    }

    public override void SetInteractOptions()
    {
        interactOptions.Add(new InteractOption() { text = "Drink", interactTime = drinkTime});
        interactOptions.Add(new InteractOption() { text = "Search for Stone", interactTime = searchTime });
    }

    public override void Interact(string interactOption)
    {
        

        if (interactOption == "Drink")
        {
            if (waterPlayer) waterPlayer.PlaySound();
            Drink();
        }
        if (interactOption == "Search for Stone")
        {
            if(stonePlayer) stonePlayer.PlaySound();
            SearchforStone();
        }
    }

    void Drink()
    {
        SurvivalAttributes.runtime.AddThirst(-SurvivalAttributes.runtime.thirst);
    }

    void SearchforStone()
    {
        PlayerInventory.runtime.AddItem(stoneItem);
    }
}
