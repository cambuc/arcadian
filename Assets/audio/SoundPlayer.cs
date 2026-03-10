using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

public class SoundPlayer : MonoBehaviour
{
    public List<AudioClip> clips;
    public AudioMixerGroup group;
    public float volume = 1;
    [Range(0,1)]
    public float spatial;

    List<AudioClip> pool = new List<AudioClip>();

    private void Awake()
    {
        foreach (AudioClip c in clips)
        {
            pool.Add(c);
        }
    }

    public void PlaySound()
    {
        AudioClip clip = pool[Random.Range(0, pool.Count)];
        pool.Clear();
        foreach(AudioClip c in clips)
        {
            if (c != clip || clips.Count <= 1) pool.Add(c);
        }
        new UniversalPlayer(clip, volume, spatial, transform.position, group);
    }
}
