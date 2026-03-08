using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class Equippable : MonoBehaviour
{
    public Animator anim;
    public Guid id { get; set; }

    private void OnValidate()
    {
        if (anim == null)
            anim = GetComponent<Animator>();
    }

    public virtual void Equip(UnityAction onEquipped) { }
    public virtual void Unequip(UnityAction onUnequipped) { }

    public async void CallOnAnimationEnd(UnityAction onEnd)
    {
        await Task.Delay(10);
        await Task.Delay((int)(anim.GetCurrentAnimatorClipInfo(0)[0].clip.length * 1000));
        onEnd.Invoke();
    }
}
