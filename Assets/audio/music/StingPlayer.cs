using System.Collections.Generic;
using UnityEngine;

public class StingPlayer : MonoBehaviour
{
    public List<AudioClip> stings = new List<AudioClip>();
    public Vector2 timeBetweenStingsRandRange;

    public AudioReverbPreset reverb;

    float timer;

    private void Update()
    {
        timer -= Time.deltaTime;

        if(timer <= 0)
        {
            timer = Random.Range(timeBetweenStingsRandRange.x, timeBetweenStingsRandRange.y);

            new UniversalPlayer(stings[Random.Range(0, stings.Count)], reverb);
        }
    }
}
