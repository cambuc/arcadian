using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractOptionsMenu : MonoBehaviour
{
    public static InteractOptionsMenu runtime;

    public RectTransform root;

    public TextMeshProUGUI txtName;
    public Transform optionsLayout;
    public InteractOptionUI optionPrefab;

    Interactable interactable;

    List<InteractOptionUI> temp = new List<InteractOptionUI>();
    int selectedIndex;

    private void Awake()
    {
        runtime = this;
    }

    public void OpenOptions(Interactable interactable)
    {
        if (interactable.interactOptions.Count <= 0)
            return;

        this.interactable = interactable;
        txtName.text = interactable.interactName;

        while(selectedIndex >= interactable.interactOptions.Count)
        {
            selectedIndex--;
        }

        //root.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, baseHeight + (interactable.interactOptions.Count * optionHeight));

        FillLayout();

        root.gameObject.SetActive(true);
    }

    void FillLayout()
    {
        ClearLayout();
        if (selectedIndex >= interactable.interactOptions.Count)
            selectedIndex = interactable.interactOptions.Count - 1;
        foreach (InteractOption i in interactable.interactOptions)
        {
            GameObject instance = Instantiate(optionPrefab.gameObject);
            InteractOptionUI option = instance.GetComponent<InteractOptionUI>();
            temp.Add(option);

            instance.transform.parent = optionsLayout;
            instance.transform.localScale = Vector3.one;
            if(i.text == "")
                option.txtName.text = i.text;
            else
                option.txtName.text = i.text;
            option.types = i;
        }
        HighlightOption();
    }
    void ClearLayout()
    {
        for (int i = 0; i < temp.Count; i++)
            Destroy(temp[i].gameObject);
        temp.Clear();
    }

    public string GetSelectedOptionString()
    {
        return interactable.interactOptions[selectedIndex].text;
    }
    public InteractOptionUI GetSelectedOption()
    {
        return temp.Count > 0 ? temp[selectedIndex] : null;
    }

    public void CloseOptions()
    {
        root.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.mouseScrollDelta.y > 0 && root.gameObject.activeSelf)
            ScrollUp();
        if (Input.mouseScrollDelta.y < 0 && root.gameObject.activeSelf)
            ScrollDown();
    }

    void ScrollUp()
    {
        selectedIndex--;
        if (selectedIndex < 0)
            selectedIndex = temp.Count - 1;
        HighlightOption();
    }
    void ScrollDown()
    {
        selectedIndex++;
        if (selectedIndex >= temp.Count)
            selectedIndex = 0;
        HighlightOption();
    }

    void HighlightOption()
    {
        foreach(InteractOptionUI i in temp)
        {
            i.selectedindicator.SetActive(false);
        }
        temp[selectedIndex].selectedindicator.SetActive(true);
    }
}
