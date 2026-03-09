using System.Collections.Generic;
using UnityEngine;

public class Campfire : CraftingStation
{
    public GameObject flames;

    public int secondsToStartFire;

    public List<Item> fireStarting = new List<Item>();
    public List<Item> firewood = new List<Item>();

    public int baseFireTime;
    public int fuelAddTime;
    int fireTime;

    public SoundPlayer interactSound;
    public SoundPlayer addFuelSound;

    private void Awake()
    {
        GameTick.tick.AddListener(Tick);
        TimeManager.runtime.WaitToCallLoop(new WaitCallLoop() { action=() => fireTime--, loopDuration = TimeManager.runtime.timeMult, secondsLeft = 1});
    }

    private void OnEnable()
    {
        Extinguish();
    }

    public void Extinguish()
    {
        flames.SetActive(false);

        interactOptions.Clear();
        interactOptions.Add(new InteractOption() { text = "Start Fire" });

        if (CraftingMenu.runtime.currentStation == (CraftingStation)this)
            CraftingMenu.runtime.CloseMenu();
    }

    public override void Interact(string interactOption)
    {
        interactSound.PlaySound();
        if (interactOption == "Craft")
        {
            CraftingMenu.runtime.OpenMenu(recipes, this);
        }
        else if (interactOption == "Add Fuel")
        {
            interactable = false;
            PlayerInventoryUI.runtime.OpenFilteredInventory(firewood, AddFuel);
        }
        else if(interactOption == "Start Fire")
        {
            interactable = false;
            PlayerInventoryUI.runtime.OpenFilteredInventory(fireStarting, LightFire);
        }
    }

    public void LightFire(Item implement)
    {
        if (implement == null)
        {
            interactable = true;
            return;
        }

        Tool tool = implement.GetComponent<Tool>();
        if (tool.useSound) new UniversalPlayer(tool.useSound);

        Fader.runtime.FadeOut(() =>
        {
            flames.SetActive(true);

            TimeManager.runtime.PassTime(secondsToStartFire * tool.timeMultiplier);
            fireTime = baseFireTime;

            interactOptions.Clear();
            interactOptions.Add(new InteractOption() { text = "Craft" });
            interactOptions.Add(new InteractOption() { text = "Add Fuel" });

            Fader.runtime.FadeIn(() =>
            {
                interactable = true;
            });
        });
    }

    public void AddFuel(Item fuel)
    {
        interactable = true;

        if (fuel == null)
            return;

        addFuelSound.PlaySound();

        fireTime += fuelAddTime;
        PlayerInventory.runtime.RemoveItem(fuel);
    }

    void Tick()
    {
        if(fireTime <= 0)
        {
            Extinguish();
        }
        else
        {
            flames.transform.localScale = new Vector3(
                flames.transform.localScale.x,
                Mathf.Lerp(0, 1, (float)fireTime / baseFireTime),
                flames.transform.localScale.z
                );
        }
    }
}
