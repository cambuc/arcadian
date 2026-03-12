using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MapEquip : Equippable
{
    public override void Equip(UnityAction onEquipped)
    {
        anim.Play("Equip");
        CallOnAnimationEnd(onEquipped);
    }

    public override void Unequip(UnityAction onUnequipped)
    {
        anim.Play("Unequip");
        CallOnAnimationEnd(onUnequipped);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            anim.SetBool("zoom", true);
            PlayerHud.runtime.DisableCrosshair();
        }
        else
        {
            anim.SetBool("zoom", false);
            PlayerHud.runtime.EnableCrosshair();
        }
    }
}
