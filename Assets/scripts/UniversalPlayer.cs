using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class UniversalPlayer
{
    public UniversalPlayer(AudioClip clip)
    {
        GameObject gm = new GameObject("Universal Player");
        AudioSource player = gm.AddComponent<AudioSource>();

        player.volume = 1;
        player.clip = clip;
        player.Play();

        Object.Destroy(gm, clip.length);
    }
    public UniversalPlayer(AudioClip clip, float volume)
    {
        GameObject gm = new GameObject("Universal Player");
        AudioSource player = gm.AddComponent<AudioSource>();

        player.volume = volume;
        player.clip = clip;
        player.Play();

        Object.Destroy(gm, clip.length);
    }
    public UniversalPlayer(AudioClip clip, AudioReverbPreset reverb)
    {
        GameObject gm = new GameObject("Universal Player");
        AudioSource player = gm.AddComponent<AudioSource>();

        AudioReverbFilter filter = gm.AddComponent<AudioReverbFilter>();
        filter.reverbPreset = reverb;

        player.volume = 1;
        player.clip = clip;
        player.Play();

        Object.Destroy(gm, clip.length);
    }
    public UniversalPlayer(AudioClip clip, float volume, float spatial, Vector3 location)
    {
        GameObject gm = new GameObject("Universal Player");
        AudioSource player = gm.AddComponent<AudioSource>();

        player.volume = volume;
        player.clip = clip;
        player.spatialBlend = spatial;
        gm.transform.position = location;
        player.Play();

        Object.Destroy(gm, clip.length);
    }
}
