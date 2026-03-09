using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnStart : MonoBehaviour
{
    public int sceneIndex;

    public int delay;

    public GameObject inputIndicator;

    private void Start()
    {
        if (inputIndicator) inputIndicator.SetActive(false);
        Delay();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && inputIndicator && inputIndicator.activeSelf)
        {
            Fader.runtime.FadeOut(() =>
            {
                SceneManager.LoadScene(sceneIndex);
            });
        }
    }

    async void Delay()
    {
        await Task.Delay(delay);
        if (inputIndicator) inputIndicator.SetActive(true);
        else SceneManager.LoadScene(sceneIndex);
    }
}
