using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public bool interactable = true;

    public string interactName;
    public string extraText;
    public Item item;

    public List<InteractOption> interactOptions = new List<InteractOption>();
    public float interactedTime { get; set; }

    [HideInInspector]
    public bool withinZone = false;
    [HideInInspector]
    public bool interacted = false;

    private void Start()
    {
        SetInteractOptions();
    }

    public virtual void SetInteractOptions()
    {
    }

    public virtual void Interact(string interactOption)
    {
        if (interactOption == "Grab")
        {
            Grab();
        }
        else if (interactOption == "Open")
        {
            Open();
        }
        else if (interactOption == "Wear")
        {
            Wear();
        }

    }

    public void Wear()
    {
        PlayerInteract.runtime.OnExitZone();
        GameObject i = Instantiate(item.gameObject);
        PlayerApparel.runtime.WearClothing(i.GetComponent<Clothing>());
        Destroy(gameObject);
    }

    public void Open()
    {
        //ContainerUI.runtime.OpenContainer((Container)this);
    }

    Transform parent;
    bool grabbing;

    public void Grab()
    {
        parent = transform.parent;
        grabbing = true;
        transform.parent = Camera.main.transform;
        interactable = false;

        PlayerInventory.runtime.grabbingItem = item;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = true;
        }
    }

    private void Update()
    {
        if (!grabbing)
            return;

        if (Input.GetKey(KeyCode.Mouse0))
        {
            transform.eulerAngles += new Vector3(0, 100 * Time.deltaTime, 0);
        }
        else if (Input.GetKey(KeyCode.Mouse1))
        {
            transform.eulerAngles -= new Vector3(0, 100 * Time.deltaTime, 0);
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            transform.parent = parent;
            grabbing = false;
            interactable = true;

            PlayerInventory.runtime.grabbingItem = null;

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb)
            {
                rb.isKinematic = false;
            }
        }

        OnUpdate();
    }

    public virtual void OnUpdate() { }
}
