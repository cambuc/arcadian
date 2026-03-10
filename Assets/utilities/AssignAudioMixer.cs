using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AssignAudioMixer : MonoBehaviour
{
    public bool ASSIGN_ALL_SOURCES;

    public AudioMixerGroup mixerGroup;

    private void OnValidate()
    {
        if (ASSIGN_ALL_SOURCES)
        {
            ASSIGN_ALL_SOURCES = false;
            AssignAllSources();
        }
    }

    void AssignAllSources()
    {
        foreach(AudioSource source in FindObjectsByType<AudioSource>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            source.outputAudioMixerGroup = mixerGroup;
        }
    }
}
