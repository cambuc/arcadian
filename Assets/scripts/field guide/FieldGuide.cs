using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FieldGuide : MonoBehaviour
{
    public static FieldGuide runtime;

    public List<FieldGuideSpecies> species = new List<FieldGuideSpecies>();
    public GameObject root;
    public FieldGuideEntry entryPrefab;
    public Button btnClose;

    public TextMeshProUGUI lblTotalDiscovered;
    public Transform entriesLayout;
    public Scrollbar scrollbar;

    public GameObject detailsRoot;
    public RawImage imgIllustration;
    public TextMeshProUGUI lblCommonName;
    public TextMeshProUGUI lblLatinName;
    public TextMeshProUGUI lblDescription;

    List<GameObject> temp = new List<GameObject>();
    FieldGuideEntry currentSelection;

    private void Awake()
    {
        runtime = this;

        btnClose.onClick.AddListener(CloseGuide);
    }

    private void Start()
    {
        foreach (FieldGuideSpecies sp in species)
            sp.discovered = false;

        CloseGuide();
    }

    public void OpenGuide()
    {
        PlayerMovement.LockMovement(gameObject, true);

        lblCommonName.text = "";
        lblLatinName.text = "";
        lblDescription.text = "";
        imgIllustration.gameObject.SetActive(false);
        detailsRoot.SetActive(false);

        currentSelection = null;

        lblTotalDiscovered.text = $"Discovered {species.Where(s => s.discovered).Count()}/{species.Count}";

        FillLayout();
        root.SetActive(true);
        scrollbar.value = 1;
    }
    public void CloseGuide()
    {
        PlayerMovement.LockMovement(gameObject, false);

        if(root.activeSelf) SoundPlayerHolder.runtime.uiClick.PlaySound();

        root.SetActive(false);
    }

    void FillLayout()
    {
        ClearTemp();
        foreach(FieldGuideSpecies sp in species.OrderBy(s => s.commonName).OrderByDescending(s => s.discovered))
        {
            FieldGuideEntry entry = Instantiate(entryPrefab.gameObject, entriesLayout).GetComponent<FieldGuideEntry>();

            entry.btnMain.onClick.AddListener(() => SelectEntry(entry));
            entry.Initialize(sp);

            temp.Add(entry.gameObject);
        }
    }

    void SelectEntry(FieldGuideEntry entry)
    {
        if (!entry.species.discovered)
            return;

        if(currentSelection) currentSelection.Deselect();
        currentSelection = entry;

        if (entry)
        {
            detailsRoot.SetActive(true);
            entry.Select();
        }

        SoundPlayerHolder.runtime.uiSelect.PlaySound();

        lblCommonName.text = $"{entry.species.commonName}";
        lblLatinName.text = $"{entry.species.latinName}";

        lblDescription.text = $"{entry.species.description}";

        imgIllustration.texture = entry.species.illustration;
        imgIllustration.gameObject.SetActive(true);
    }

    void ClearTemp()
    {
        foreach(GameObject t in temp)
        {
            Destroy(t);
        }
        temp.Clear();
    }
}
