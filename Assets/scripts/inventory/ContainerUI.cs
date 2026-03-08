using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContainerUI : MonoBehaviour
{
    public static ContainerUI runtime;

    public GameObject root;

    public Transform itemsLayout;
    public ContainerUIItem itemPrefab;
    public Button btnClose;

    Container container;
    List<ContainerUIItem> temp = new List<ContainerUIItem>();

    private void Awake()
    {
        runtime = this;

        btnClose.onClick.AddListener(CloseContainer);
    }

    private void Start()
    {
        root.SetActive(false);
    }

    public void OpenContainer(Container container)
    {
        PlayerMovement.LockMovement(gameObject, true);
        this.container = container;

        FillLayout();
        root.SetActive(true);
    }

    void FillLayout()
    {
        ClearLayout();
        foreach (Item i in container.items)
        {
            GameObject instance = Instantiate(itemPrefab.gameObject);
            ContainerUIItem item = instance.GetComponent<ContainerUIItem>();
            temp.Add(item);

            item.item = i;
            instance.transform.SetParent(itemsLayout);
            item.txtName.text = i.name;
            item.btnRemove.onClick.AddListener(() => RemoveItem(item));
        }
    }
    void RemoveItem(ContainerUIItem item)
    {
        container.items.Remove(item.item);
        item.item.Drop();
        FillLayout();
    }

    void ClearLayout()
    {
        for (int i = 0; i < temp.Count; i++)
            Destroy(temp[i].gameObject);
        temp.Clear();
    }

    void CloseContainer()
    {
        PlayerMovement.LockMovement(gameObject, false);

        root.SetActive(false);
    }
}
