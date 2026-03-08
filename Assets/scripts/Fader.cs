using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    public static Fader runtime;

    private void Awake()
    {
        runtime = this;
    }

    public Image fader;
    public float defaultFadeTime = 0.5f;

    private void Start()
    {
        FadeIn();
    }

    public async Task FadeOut(float time)
    {
        for(float i = 0; i < time; i += 0.025f)
        {
            fader.color = new Color(fader.color.r, fader.color.g, fader.color.b, Mathf.Lerp(0, 1, i / time));
            await Task.Delay(25);
        }
    }
    public async Task FadeIn(float time)
    {
        for (float i = 0; i < time; i += 0.025f)
        {
            fader.color = new Color(fader.color.r, fader.color.g, fader.color.b, Mathf.Lerp(1, 0, i / time));
            await Task.Delay(25);
        }
    }

    public async void FadeOut() { await FadeOut(defaultFadeTime); }
    public async void FadeIn() { await FadeIn(defaultFadeTime); }

    public async void FadeOut(UnityAction onComplete) 
    { 
        await FadeOut(defaultFadeTime);
        onComplete.Invoke();
    }
    public async void FadeIn(UnityAction onComplete)
    {
        await FadeIn(defaultFadeTime);
        onComplete.Invoke();
    }
}
