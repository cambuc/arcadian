using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class HUDMessage : MonoBehaviour
{
    public static HUDMessage runtime;

    public GameObject root;
    public TextMeshProUGUI label;

    public int defaultLength;

    private void Awake()
    {
        runtime = this;
    }

    private void Start()
    {
        label.text = "";
        root.SetActive(false);
    }

    public void ShowMessage(string message)
    {
        ShowMessage(message, defaultLength);
    }
    public async void ShowMessage(string message, int length)
    {
        label.text = message;
        root.SetActive(true);
        await Task.Delay(length);
        label.text = "";
        root.SetActive(false);
    }
}
