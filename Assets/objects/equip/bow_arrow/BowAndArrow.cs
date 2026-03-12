using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class BowAndArrow : Equippable
{
    public GameObject arrow;
    public Item arrowItem;
    public FlyingArrow flyingArrow;
    public float damage;

    public SoundPlayer equipSound;
    public SoundPlayer drawSound;
    public SoundPlayer shootSound;

    private void Awake()
    {
        PlayerInventory.onInventoryChanged.AddListener(OnInventoryChanged);
    }

    public override void Equip(UnityAction onEquipped)
    {
        CheckArrows();

        equipSound.PlaySound();
        anim.Play("Equip");
        CallOnAnimationEnd(onEquipped);
    }

    public override void Unequip(UnityAction onUnequipped)
    {
        anim.Play("Unequip");
        CallOnAnimationEnd(onUnequipped);
    }

    public void Draw()
    {
        anim.Play("Draw");
        drawSound.PlaySound();
    }

    void Release()
    {
        anim.Play("Release");
        shootSound.PlaySound();
    }

    public void InstantiateArrow()
    {
        if (!CanShoot())
            return;

        PlayerInventory.runtime.RemoveItem(arrowItem);
        CheckArrows();

        FlyingArrow flying = Instantiate(flyingArrow.gameObject).GetComponent<FlyingArrow>();
        flying.transform.position = arrow.transform.position;
        flying.transform.eulerAngles = arrow.transform.eulerAngles;
        flying.transform.localScale = arrow.transform.lossyScale;
        flying.damage = damage;
        flying.Shoot();

        FaunaBehavior.spookEvent.Invoke(false);
    }

    void OnInventoryChanged()
    {
        if (PlayerInventory.runtime.HasItem(arrowItem) && arrow.activeSelf == false)
        {
            equipSound.PlaySound();
            anim.Play("Equip");
            arrow.SetActive(true);
        }
    }
    void CheckArrows()
    {
        if (!PlayerInventory.runtime.HasItem(arrowItem))
            arrow.SetActive(false);
        else
            arrow.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0) && anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Idle" && CanAnimate() && CanShoot())
        {
            Draw();
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if(anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "DrawHold" && CanAnimate() && CanShoot()) Release();
            else
            {
                anim.Play("Idle");
            }
        }
    }

    bool CanAnimate()
    {
        AnimationClip current = anim.GetCurrentAnimatorClipInfo(0)[0].clip;
        return !PlayerMovement.IsMovementLocked() && PlayerEquip.runtime.IsEquipped(this);
    }
    bool CanShoot()
    {
        return PlayerInventory.runtime.HasItem(arrowItem);
    }
}
