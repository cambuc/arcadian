using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu runtime;

    public GameObject root;

    public Button btnContinue;
    public Button btnQuit;

    float inputTimer;

    private void Awake()
    {
        runtime = this;
    }

    private void Start()
    {
        Unpause();

        btnContinue.onClick.AddListener(() =>
        {
            Unpause();
        });
        btnQuit.onClick.AddListener(() =>
        {
            Fader.runtime.FadeOut(() =>
            {
                Application.Quit();
            });
        });
    }

    private void Update()
    {
        inputTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Escape) && inputTimer <= 0 && !PlayerMovement.IsMovementLocked() && !BuildingMenu.runtime.root.activeSelf)
        {
            inputTimer = 0.25f;
            Pause();
        }
        if (Input.GetKeyDown(KeyCode.Escape) && inputTimer <= 0)
        {
            inputTimer = 0.25f;
            Unpause();
        }
    }

    public void Pause()
    {
        PlayerMovement.LockMovement(gameObject, true);
        root.SetActive(true);

        GameTick.runtime.Pause();
        Time.timeScale = 0;
    }

    public void Unpause()
    {
        PlayerMovement.LockMovement(gameObject, false);
        root.SetActive(false);

        GameTick.runtime.Unpause();
        Time.timeScale = 1;
    }
}
