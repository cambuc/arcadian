using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DeerBehavior : FaunaBehavior
{
    public List<Collider> activeColliders = new List<Collider>();
    public List<Collider> downedColliders = new List<Collider>();

    private void OnEnable()
    {
        foreach(Collider col in activeColliders)
            col.enabled = true;
        foreach (Collider col in downedColliders)
            col.enabled = false;
    }

    public override void IdleState()
    {
        if (GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name != "idle")
            GetComponent<Animator>().Play("idle");
    }
    public override void RoamState()
    {
        if (GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name != "walk")
            GetComponent<Animator>().Play("walk");
    }
    public override void FleeState()
    {
        if (GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name != "run")
            GetComponent<Animator>().Play("run");
    }
    public override void LookAnimate(Vector3 target)
    {

    }
    public override void AnimateDeath()
    {
        if (GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name != "death")
            GetComponent<Animator>().Play("death");

        foreach (Collider col in activeColliders)
            col.enabled = false;
        foreach (Collider col in downedColliders)
            col.enabled = true;
    }
}
