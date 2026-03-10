using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MixerGroupHolder : MonoBehaviour
{
    public static MixerGroupHolder runtime;

    public AudioMixerGroup master;
    public AudioMixerGroup music;
    public AudioMixerGroup ambience;
    public AudioMixerGroup sfx;
    public AudioMixerGroup ui;

    private void Awake()
    {
        runtime = this;
    }
}
