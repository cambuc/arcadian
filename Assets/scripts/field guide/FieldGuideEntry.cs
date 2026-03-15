using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FieldGuideEntry : MonoBehaviour
{
    public Image imgMain;
    public Button btnMain;
    public TextMeshProUGUI lblMain;

    public Color defaultColor;
    public Color selectedColor;
    public Color undiscoveredColor;

    public FieldGuideSpecies species { get; set; }

    public void Initialize(FieldGuideSpecies species)
    {
        this.species = species;

        lblMain.text = species.commonName;

        if (!species.discovered)
            imgMain.color = undiscoveredColor;
        else
            imgMain.color = defaultColor;
    }

    public void Select()
    {
        imgMain.color = selectedColor;
    }
    public void Deselect()
    {
        imgMain.color = defaultColor;
    }
}
