using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonShortcut : MonoBehaviour
{
    public List<string> shortcuts = new List<string>();
    public bool any = false;

    private void Update()
    {
        if (any)
        {
            if (Input.anyKeyDown)
            {
                GetComponent<Button>().onClick.Invoke();
            }
        }
        else
        {
            foreach (string shortcut in shortcuts)
            {
                if (Input.GetKeyDown(shortcut))
                {
                    GetComponent<Button>().onClick.Invoke();
                }
            }
        }
    }
}
