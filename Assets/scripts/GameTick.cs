using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class GameTick : MonoBehaviour
{
    public static GameTick runtime;

    public static UnityEvent tick = new UnityEvent();

    public int tickLength = 1000;

    private void Awake()
    {
        runtime = this;
    }

    private void Start()
    {
        Tick();
    }

    async void Tick()
    {
        while (Application.isPlaying)
        {
            tick.Invoke();
            await Task.Delay(tickLength);
        }
    }
}
