using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class ThrowingStick : Equippable
{
    public Item throwingStickItem;
    public Transform thrownAlignment;
    public ThrownStick thrownPrefab;
    public float damage;
    public SoundPlayer stickThrow;

    bool throwing;

    public override void Equip(UnityAction onEquipped)
    {
        anim.Play("Equip");
        CallOnAnimationEnd(onEquipped);
    }

    public override void Unequip(UnityAction onUnequipped)
    {
        if (throwing)
        {
            throwing = false;
            onUnequipped.Invoke();
            return;
        }

        anim.Play("Unequip");
        CallOnAnimationEnd(onUnequipped);
    }

    public void Draw()
    {
        anim.Play("Draw");
    }

    void Release()
    {
        stickThrow.PlaySound();
        anim.Play("Release");
    }

    public void InstantiateThrown()
    {
        throwing = true;
        PlayerEquip.runtime.Unequip(this);
        PlayerInventory.runtime.RemoveItem(throwingStickItem);

        ThrownStick thrown = Instantiate(thrownPrefab.gameObject).GetComponent<ThrownStick>();
        thrown.transform.position = thrownAlignment.transform.position;
        thrown.transform.eulerAngles = thrownAlignment.transform.eulerAngles;
        thrown.transform.localScale = thrownAlignment.transform.lossyScale;
        thrown.damage = damage;
        thrown.Shoot();

        FaunaBehavior.spookEvent.Invoke(false);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0) && anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Idle" && CanAnimate())
        {
            Draw();
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if(anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "DrawHold" && CanAnimate()) Release();
            else
            {
                anim.Play("Idle");
            }
        }
    }

    bool CanAnimate()
    {
        return !PlayerMovement.IsMovementLocked() && PlayerEquip.runtime.IsEquipped(this);
    }
}
