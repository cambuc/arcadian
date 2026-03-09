using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathScreen : MonoBehaviour
{
    public static DeathScreen runtime;

    public GameObject root;
    public TextMeshProUGUI lblMessage;

    public Button btnRetry;
    public Button btnQuit;

    public int deathSceneIndex = 2;

    private void Awake()
    {
        runtime = this;
    }

    private void Start()
    {
        root.SetActive(false);
        onScreen = false;
        PlayerMovement.LockMovement(gameObject, false);

        btnRetry.onClick.AddListener(() =>
        {
            Fader.runtime.FadeOut(() =>
            {
                PlayerMovement.LockMovement(gameObject, false);
                SceneManager.LoadSceneAsync(deathSceneIndex);
            });
        });
        btnQuit.onClick.AddListener(() =>
        {
            Fader.runtime.FadeOut(() =>
            {
                Application.Quit();
            });
        });
    }

    bool onScreen;
    public void ShowDeathScreen(string message)
    {
        if (onScreen) return;

        onScreen = true;

        PlayerMovement.LockMovement(gameObject, true);
        AudioListener.volume = 0;
        TimeManager.runtime.timeMult = 1;
        Fader.runtime.FadeOut(() =>
        {
            root.SetActive(true);
            lblMessage.text = message;
            Fader.runtime.FadeIn();
        });
    }
}
