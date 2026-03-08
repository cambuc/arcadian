using System.Collections.Generic;
using UnityEngine;

public class BuildingPreview : MonoBehaviour
{
    public BuildingRecipe recipe;
    public float rotationSpeed;

    Collider col;
    Rigidbody rb;

    List<GameObject> hiddenDuringPreview = new List<GameObject>();

    public void SetValues(BuildingRecipe recipe, float rotationSpeed)
    {
        this.recipe = recipe;
        this.rotationSpeed = rotationSpeed;

        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();

        if (col) col.enabled = false;
        if (rb) rb.isKinematic = true;
        foreach(Collider childrenColliders in GetComponentsInChildren<Collider>())
        {
            if (childrenColliders.gameObject == gameObject)
                continue;

            childrenColliders.gameObject.SetActive(false);
            hiddenDuringPreview.Add(childrenColliders.gameObject);
        }
    }

    void Update()
    {
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

        if (Input.GetKey(KeyCode.Mouse0))
        {
            transform.eulerAngles += new Vector3(0, Time.deltaTime * rotationSpeed, 0);
        }
        if (Input.GetKey(KeyCode.Mouse1))
        {
            transform.eulerAngles += new Vector3(0, Time.deltaTime * -rotationSpeed, 0);
        }

        if (!BuildingMenu.runtime.groundPreview)
            return;

        Physics.Raycast(new Ray(transform.position + Vector3.up * 100, Vector3.down), out RaycastHit hit, 1000, LayerMask.GetMask("Ground"));
        if(hit.transform)
            transform.position = new Vector3(transform.position.x, hit.point.y + recipe.groundHeightOffset, transform.position.z);
    }

    public void Place()
    {
        if (col) col.enabled = true;
        if (rb) rb.isKinematic = false;

        transform.parent = null;

        foreach (GameObject hidden in hiddenDuringPreview)
        {
            hidden.SetActive(true);
        }
    }
}
