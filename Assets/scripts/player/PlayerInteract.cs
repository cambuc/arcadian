using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerInteract : MonoBehaviour
{
    public static PlayerInteract runtime;

    public Camera playerCam;

    public GameObject interactionIndicator;

    public Slider interactSlider;

    public float interactRadius;

    public float rayForwardOffset;

    Interactable currentInst;

    private void Awake()
    {
        runtime = this;
    }
    private void Start()
    {
        OnExitZone();
    }

    float timer;
    void Update()
    {
        if (PlayerMovement.IsMovementLocked())
        {
            OnExitZone();
            return;
        }

        Physics.Raycast(
            new Ray(
                playerCam.transform.position + playerCam.transform.forward * rayForwardOffset, 
                playerCam.transform.forward), 
            out RaycastHit hit, interactRadius);

        if (!hit.transform || !hit.transform.GetComponent<Interactable>())
        {
            OnExitZone();
            return;
        }

        Interactable inst = hit.transform ? hit.transform.GetComponent<Interactable>() : null;
        inst = inst == null ? (hit.transform && hit.transform.parent ? hit.transform.parent.GetComponent<Interactable>() : inst) : inst;
        if (inst != null && inst.interactable)
        {
            if(inst != currentInst)
            {
                OnEnterZone(inst);
            }

            if (InteractOptionsMenu.runtime.GetSelectedOption().types.interactTime > 0)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    timer -= 0.1f;
                    interactSlider.gameObject.SetActive(true);
                }
                if (Input.GetKey(KeyCode.E) && timer < InteractOptionsMenu.runtime.GetSelectedOption().types.interactTime)
                {
                    timer -= Time.deltaTime;
                    interactSlider.gameObject.SetActive(true);
                }
                else if (Input.GetKeyUp(KeyCode.E))
                {
                    inst.interactedTime = InteractOptionsMenu.runtime.GetSelectedOption().types.interactTime - timer;
                }
                else
                {
                    timer = InteractOptionsMenu.runtime.GetSelectedOption().types.saveTime ? 
                        InteractOptionsMenu.runtime.GetSelectedOption().types.interactTime - inst.interactedTime : 
                        InteractOptionsMenu.runtime.GetSelectedOption().types.interactTime;
                    interactSlider.gameObject.SetActive(false);
                }
                interactSlider.value = Mathf.Abs((timer / InteractOptionsMenu.runtime.GetSelectedOption().types.interactTime) - 1);
            }

            if ((Input.GetKeyDown(KeyCode.E) && InteractOptionsMenu.runtime.GetSelectedOption().types.interactTime == 0)
                || (timer <= 0 && InteractOptionsMenu.runtime.GetSelectedOption().types.interactTime > 0))
            {
                timer = InteractOptionsMenu.runtime.GetSelectedOption().types.interactTime;
                inst.Interact(InteractOptionsMenu.runtime.GetSelectedOptionString());
                OnExitZone();
            }
        }
        else if (currentInst != null)
        {
            OnExitZone();
        }
    }

    void OnEnterZone(Interactable inst)
    {
        currentInst = inst;

        interactionIndicator.SetActive(true);
        InteractOptionsMenu.runtime.OpenOptions(inst);

        timer = InteractOptionsMenu.runtime.GetSelectedOption().types.interactTime;
        if (InteractOptionsMenu.runtime.GetSelectedOption().types.interactTime <= 0)
        {
            interactSlider.gameObject.SetActive(false);
        }
    }
    public void OnExitZone()
    {
        currentInst = null;
        interactionIndicator.SetActive(false);
        InteractOptionsMenu.runtime.CloseOptions();
        interactSlider.gameObject.SetActive(false);
    }
    public void OnExitZone(UnityAction onExit)
    {
        currentInst = null;
        interactionIndicator.SetActive(false);
        InteractOptionsMenu.runtime.CloseOptions();
        interactSlider.gameObject.SetActive(false);
        onExit();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(
            playerCam.transform.position + playerCam.transform.forward * rayForwardOffset,
            playerCam.transform.position + playerCam.transform.forward * interactRadius
            );
    }
}
