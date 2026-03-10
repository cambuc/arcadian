using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FootstepSounder : MonoBehaviour
{
    public List<AudioClip> stepSounds = new List<AudioClip>();
    public AudioClip landSound;

    public AudioSource source;
    public TerrainCollider terrainCollider; 
    public float stepLength;

    List<AudioClip> pool = new List<AudioClip>();
    float timer;

    private void Awake()
    {
        PlayerMovement.onLanded.AddListener(OnLanded);
    }

    bool IsWalking()
    {
        return !PlayerMovement.IsMovementLocked() && PlayerMovement.runtime.IsGrounded() && 
            (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0);
    }

    void Update()
    {
        if (!IsWalking())
            return;

        timer -= Time.deltaTime;
        if (timer > 0)
            return;

        timer = stepLength / (PlayerMovement.runtime.GetSpeed() / Time.deltaTime);

        pool.Clear();
        foreach (AudioClip c in stepSounds)
            if (c != source.clip)
                pool.Add(c);

        source.clip = pool[Random.Range(0, pool.Count)];
        source.Play();
    }

    bool IsIndoor()
    {
        return PlayerMovement.runtime.GetOverlaps()[0] != terrainCollider;
    }

    void OnLanded()
    {
        new UniversalPlayer(landSound, MixerGroupHolder.runtime.sfx);
    }
}
