using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

public class UniversalPlayer
{
    public UniversalPlayer(AudioClip clip, AudioMixerGroup group)
    {
        GameObject gm = new GameObject("Universal Player");
        AudioSource player = gm.AddComponent<AudioSource>();

        player.volume = 1;
        player.clip = clip;
        player.outputAudioMixerGroup = group;
        player.Play();

        Object.Destroy(gm, clip.length);
    }
    public UniversalPlayer(AudioClip clip, float volume, AudioMixerGroup group)
    {
        GameObject gm = new GameObject("Universal Player");
        AudioSource player = gm.AddComponent<AudioSource>();

        player.volume = volume;
        player.clip = clip;
        player.outputAudioMixerGroup = group;
        player.Play();

        Object.Destroy(gm, clip.length);
    }
    public UniversalPlayer(AudioClip clip, AudioReverbPreset reverb, AudioMixerGroup group)
    {
        GameObject gm = new GameObject("Universal Player");
        AudioSource player = gm.AddComponent<AudioSource>();

        AudioReverbFilter filter = gm.AddComponent<AudioReverbFilter>();
        filter.reverbPreset = reverb;

        player.volume = 1;
        player.clip = clip;
        player.outputAudioMixerGroup = group;
        player.Play();

        Object.Destroy(gm, clip.length);
    }
    public UniversalPlayer(AudioClip clip, float volume, float spatial, Vector3 location, AudioMixerGroup group)
    {
        GameObject gm = new GameObject("Universal Player");
        AudioSource player = gm.AddComponent<AudioSource>();

        player.volume = volume;
        player.clip = clip;
        player.outputAudioMixerGroup = group;
        player.spatialBlend = spatial;
        gm.transform.position = location;
        player.Play();

        Object.Destroy(gm, clip.length);
    }
}
