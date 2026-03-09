using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class GameTick : MonoBehaviour
{
    public static GameTick runtime;

    public static UnityEvent tick = new UnityEvent();

    public int tickLength = 1000;

    public bool paused { get; private set; }

    private void Awake()
    {
        runtime = this;
    }

    private void Start()
    {
        Tick();
    }

    bool runningTick;
    async void Tick()
    {
        if (runningTick)
            return;

        runningTick = true;
        while (Application.isPlaying && !paused)
        {
            tick.Invoke();
            await Task.Delay(tickLength);
        }
    }

    public void Pause()
    {
        paused = true;
        runningTick = false;
    }
    public void Unpause()
    {
        paused = false;
        Tick();
    }
}
