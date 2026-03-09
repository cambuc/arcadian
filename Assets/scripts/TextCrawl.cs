using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class TextCrawl : MonoBehaviour
{
    public TextMeshProUGUI label;

    public int delay = 10;

    string text;

    private void Start()
    {
        text = label.text;
        label.text = "";
        Crawl();
    }

    async void Crawl()
    {
        foreach(char c in text)
        {
            label.text += c;
            await Task.Delay(delay);
        }
    }
}
