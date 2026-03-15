using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Campfire : CraftingStation
{
    public GameObject flames;
    public TemperatureZone tempZone;
    public float fireTemperature;

    public int secondsToStartFire;

    public List<Item> fireStarting = new List<Item>();
    public List<Item> fireRestarting = new List<Item>();

    [System.Serializable]
    public struct Firewood
    {
        public Item fuel;
        public float burnTimeSec;
    }
    public List<Firewood> firewood = new List<Firewood>();

    public float baseFireTimeScnds;
    public float fuelAddTime;
    public float maxFireTime;
    public float fireTime { get; private set; }

    public SoundPlayer interactSound;
    public SoundPlayer addFuelSound;

    [Header("Cooking")]
    public CookingProduct cookingMeatPrefab;
    public float distanceCooking;
    public float distanceDrying;
    public float spacing;

    bool extinguished;

    private void Awake()
    {
        GameTick.tick.AddListener(Tick);
        TimeManager.timePassed.AddListener((float timePassed) => { fireTime -= timePassed; });
    }

    public override void SetInteractOptions()
    {
    }

    private void OnEnable()
    {
        flames.SetActive(false);
        tempZone.temperatureModifier = 0;
        extinguished = true;

        interactOptions.Clear();
        interactOptions.Add(new InteractOption() { text = "Start Fire" });
        interactOptions.Add(new InteractOption() { text = "Tear Down", interactTime = 0.5f });

        if (CraftingMenu.runtime.currentStation == (CraftingStation)this)
            CraftingMenu.runtime.CloseMenu();
    }

    public void Extinguish()
    {
        flames.SetActive(false);
        tempZone.temperatureModifier = 0;
        extinguished = true;

        interactOptions.Clear();
        interactOptions.Add(new InteractOption() { text = "Restart Fire" });
        interactOptions.Add(new InteractOption() { text = "Tear Down", interactTime = 0.5f });

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
            List<Item> includes = new List<Item>();
            foreach(Firewood f in firewood)
            {
                includes.Add(f.fuel);
            }
            PlayerInventoryUI.runtime.OpenFilteredInventory(includes, AddFuel);
        }
        else if(interactOption == "Start Fire")
        {
            interactable = false;
            PlayerInventoryUI.runtime.OpenFilteredInventory(fireStarting, LightFire);
        }
        else if (interactOption == "Restart Fire")
        {
            interactable = false;
            PlayerInventoryUI.runtime.OpenFilteredInventory(fireRestarting, RelightFire);
        }
        else if(interactOption == "Tear Down")
        {
            Destroy(gameObject);
        }
    }

    public void LightFire(Item implement)
    {
        if (implement == null)
        {
            interactable = true;
            return;
        }

        Tool tool = (Tool)implement;
        tool.LoseCondition();
        if (tool.useSound) new UniversalPlayer(tool.useSound, MixerGroupHolder.runtime.sfx);

        Fader.runtime.FadeOut(() =>
        {
            flames.SetActive(true);
            tempZone.temperatureModifier = fireTemperature;
            extinguished = false;

            TimeManager.runtime.PassTime(secondsToStartFire * tool.timeMultiplier);
            fireTime = baseFireTimeScnds;

            interactOptions.Clear();
            interactOptions.Add(new InteractOption() { text = "Craft" });
            interactOptions.Add(new InteractOption() { text = "Add Fuel" });
            interactOptions.Add(new InteractOption() { text = "Tear Down", interactTime = 0.5f });

            Fader.runtime.FadeIn(() =>
            {
                interactable = true;
            });
        });
    }
    public void RelightFire(Item kindling)
    {
        if (kindling == null)
        {
            interactable = true;
            return;
        }

        PlayerInventory.runtime.RemoveItem(kindling);

        Fader.runtime.FadeOut(() =>
        {
            flames.SetActive(true);
            tempZone.temperatureModifier = fireTemperature;
            extinguished = false;

            TimeManager.runtime.PassTime(secondsToStartFire);
            fireTime = baseFireTimeScnds;

            interactOptions.Clear();
            interactOptions.Add(new InteractOption() { text = "Craft" });
            interactOptions.Add(new InteractOption() { text = "Add Fuel" });
            interactOptions.Add(new InteractOption() { text = "Tear Down", interactTime = 0.5f });

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

        Firewood fw = firewood.Find(f => f.fuel.itemName == fuel.itemName);

        addFuelSound.PlaySound();

        if(fireTime <= maxFireTime)
            fireTime += fw.burnTimeSec;
        PlayerInventory.runtime.RemoveItem(fuel);
    }

    void Tick()
    {
        if(fireTime <= 0)
        {
            extraText = "";
            if (!extinguished) Extinguish();
        }
        else
        {
            float minutes = fireTime / 60f;

            string hours = "";
            if (minutes >= 60) hours = $"{(int)(minutes / 60f)} hr ";
            extraText = hours + $"{(int)(minutes % 60f)} min";

            flames.transform.localScale = new Vector3(
                flames.transform.localScale.x,
                Mathf.Lerp(0, 1, (float)fireTime / baseFireTimeScnds),
                flames.transform.localScale.z
                );
        }
    }

    public void PlaceMeatCooking(CookingRecipe recipe)
    {
        CookingProduct stick = Instantiate(cookingMeatPrefab.gameObject, transform).GetComponent<CookingProduct>();

        stick.recipe = recipe;
        stick.campfire = this;

        float forward = Random.Range(0f, 1f);
        float right = Mathf.Sqrt(1f - forward * forward);
        Vector3 direction = new Vector3((Random.Range(0, 2) == 0 ? -1f : 1f) * right, 0, (Random.Range(0, 2) == 0 ? -1f : 1f) * forward);

        stick.transform.position = transform.position + direction * (recipe.drying ? distanceDrying : distanceCooking);
        stick.transform.LookAt(transform);

        stick.SetInteractOptions();
    }
}
