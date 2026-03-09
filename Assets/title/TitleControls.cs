using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleControls : MonoBehaviour
{
    public int sceneMainIndex;

    public GameObject root;

    public Button btnPlay;
    public Button btnSettings;
    public Button btnQuit;

    private void Awake()
    {
        btnPlay.onClick.AddListener(Play);
        btnSettings.onClick.AddListener(Settings);
        btnQuit.onClick.AddListener(Quit);
    }

    public void Play()
    {
        Fader.runtime.FadeOut(() =>
        {
            SceneManager.LoadSceneAsync(sceneMainIndex);
        });
    }
    public void Settings()
    {
        SettingsMenu.runtime.Open();
        root.SetActive(false);
    }
    public void Quit()
    {
        Fader.runtime.FadeOut(() =>
        {
            Application.Quit();
        });
    }

    private void Update()
    {
        if(!root.activeSelf && !SettingsMenu.runtime.root.activeSelf)
            root.SetActive(true);
    }
}
