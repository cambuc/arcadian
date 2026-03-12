using UnityEngine;

public class Consumable : Item
{
    public float sustenance;

    public float hoursTillRotten;

    public virtual void OnConsume()
    {
        SurvivalAttributes.runtime.AddHunger(-sustenance);
        PlayerInventory.runtime.RemoveItem(this);
        Destroy(this);
    }

    public override void OnAdd()
    {
        if(hasCondition)
            TimeManager.timePassed.AddListener(TimePassed);
    }

    void TimePassed(float sec)
    {
        condition -= sec / (hoursTillRotten * 3600f);
    }
}
