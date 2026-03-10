using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public static SettingsMenu runtime;

    public GameObject root;

    public SoundPlayer uiSelect;
    public SoundPlayer uiClick;

    [Header("Audio")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider ambienceVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider uiVolumeSlider;

    public Button btnClose;

    private void Awake()
    {
        runtime = this;

        btnClose.onClick.AddListener(Close);
        btnClose.onClick.AddListener(uiClick.PlaySound);
    }

    private void Start()
    {

        //Volume Sliders
        masterVolumeSlider.value = VolumeFromMixer("MasterVolume") * masterVolumeSlider.maxValue;
        masterVolumeSlider.onValueChanged.AddListener((float volume) => 
        { 
            VolumeChanged(volume, masterVolumeSlider, "MasterVolume"); 
        });

        musicVolumeSlider.value = VolumeFromMixer("MusicVolume") * musicVolumeSlider.maxValue;
        musicVolumeSlider.onValueChanged.AddListener((float volume) =>
        {
            VolumeChanged(volume, musicVolumeSlider, "MusicVolume");
        });

        ambienceVolumeSlider.value = VolumeFromMixer("AmbienceVolume") * ambienceVolumeSlider.maxValue;
        ambienceVolumeSlider.onValueChanged.AddListener((float volume) =>
        {
            VolumeChanged(volume, ambienceVolumeSlider, "AmbienceVolume");
        });

        sfxVolumeSlider.value = VolumeFromMixer("SFXVolume") * sfxVolumeSlider.maxValue;
        sfxVolumeSlider.onValueChanged.AddListener((float volume) =>
        {
            VolumeChanged(volume, sfxVolumeSlider, "SFXVolume");
        });

        uiVolumeSlider.value = VolumeFromMixer("UIVolume") * uiVolumeSlider.maxValue;
        uiVolumeSlider.onValueChanged.AddListener((float volume) =>
        {
            VolumeChanged(volume, uiVolumeSlider, "UIVolume");
        });

        Close();
    }

    public float VolumeFromMixer(string parameter)
    {
        float dB;
        if (MixerGroupHolder.runtime.master.audioMixer.GetFloat(parameter, out dB))
        {
            if (dB <= -80f)
                return 0f;

            return Mathf.Pow(10f, dB / 20f);
        }
        return 1f;
    }

    public void Open()
    {
        root.SetActive(true);
    }
    public void Close()
    {
        root.SetActive(false);
    }

    void VolumeChanged(float volume, Slider slider, string parameter)
    {
        if (volume <= 0.001f)
            MixerGroupHolder.runtime.master.audioMixer.SetFloat(parameter, -80f);
        else
            MixerGroupHolder.runtime.master.audioMixer.SetFloat(parameter, Mathf.Log10(volume / slider.maxValue) * 20);
        uiSelect.PlaySound();
    }
}
