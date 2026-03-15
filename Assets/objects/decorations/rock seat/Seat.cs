using System.Collections.Generic;
using UnityEngine;

public class Seat : Interactable
{
    public Transform seatedTransform;

    public float fatigueDrainMult;

    bool seated;
    float timer;

    public override void SetInteractOptions()
    {
        interactOptions.Clear();

        interactOptions.Add(new InteractOption()
        {
            text = "Sit"
        });
        interactOptions.Add(new InteractOption()
        {
            text = "Tear Down",
            interactTime = 0.5f
        });
    }

    public override void Interact(string interactOption)
    {
        if (interactOption == "Sit")
        {
            interactable = false;
            seated = true;
            PlayerMovement.runtime.transform.position = seatedTransform.position;
            PlayerMovement.StopMovement(gameObject, true);
            PlayerMovement.runtime.col.isTrigger = true;

            SurvivalAttributes.runtime.multipliersFatigue.Add("sitting", fatigueDrainMult);

            timer = 0.25f;
        }
        else if (interactOption == "Tear Down")
        {
            Destroy(gameObject);
        }
    }

    public override void OnUpdate()
    {
        if (!seated)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0 && (Input.GetAxis("Vertical") > 0 || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.Backspace)))
            Release();
    }

    void Release()
    {
        seated = false;
        interactable = true;
        PlayerMovement.runtime.transform.position = seatedTransform.position + PlayerMovement.runtime.transform.forward;
        PlayerMovement.runtime.col.isTrigger = false;
        PlayerMovement.StopMovement(gameObject, false);

        SurvivalAttributes.runtime.multipliersFatigue.Remove("sitting");
    }
}
