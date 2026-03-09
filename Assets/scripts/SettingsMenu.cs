using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public static SettingsMenu runtime;

    public GameObject root;

    public SoundPlayer uiSelect;
    public SoundPlayer uiClick;

    public Slider volumeSlider;

    public Button btnClose;

    private void Awake()
    {
        runtime = this;

        volumeSlider.value = AudioListener.volume * volumeSlider.maxValue;
        volumeSlider.onValueChanged.AddListener(VolumeChanged);

        btnClose.onClick.AddListener(Close);
        btnClose.onClick.AddListener(uiClick.PlaySound);
    }

    private void Start()
    {
        Close();
    }

    public void Open()
    {
        root.SetActive(true);
    }
    public void Close()
    {
        root.SetActive(false);
    }

    void VolumeChanged(float volume)
    {
        AudioListener.volume = volume / volumeSlider.maxValue;
        uiSelect.PlaySound();
    }
}
