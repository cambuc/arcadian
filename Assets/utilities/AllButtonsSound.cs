using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AllButtonsSound : MonoBehaviour
{
    public SoundPlayer onClickSound;

    private void Start()
    {
        foreach(Button button in FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            button.onClick.AddListener(onClickSound.PlaySound);
        }
    }
}
