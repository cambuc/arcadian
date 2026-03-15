using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StingPlayer : MonoBehaviour
{
    public List<AudioClip> stings = new List<AudioClip>();
    public float initialWait;
    public Vector2 timeBetweenStingsRandRange;

    float timer;
    List<AudioClip> pool = new List<AudioClip>();

    public AudioSource source;

    private void Start()
    {
        timer = initialWait;
        pool = stings.ToList();
    }

    private void Update()
    {
        if(!source.isPlaying)
            timer -= Time.deltaTime;

        if(timer <= 0)
        {
            timer = Random.Range(timeBetweenStingsRandRange.x, timeBetweenStingsRandRange.y);

            source.clip = pool[Random.Range(0, pool.Count)];

            pool = stings.ToList();
            pool.Remove(source.clip);

            source.Play();
        }
    }
}
