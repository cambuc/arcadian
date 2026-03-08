using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu runtime;

    public GameObject root;

    public Button btnContinue;
    public Button btnQuit;

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
        if (Input.GetKeyDown(KeyCode.Escape) && !PlayerMovement.IsMovementLocked() && !BuildingMenu.runtime.root.activeSelf)
        {
            Pause();
        }
    }

    public void Pause()
    {
        PlayerMovement.LockMovement(gameObject, true);
        root.SetActive(true);
    }

    public void Unpause()
    {
        PlayerMovement.LockMovement(gameObject, false);
        root.SetActive(false);
    }
}
