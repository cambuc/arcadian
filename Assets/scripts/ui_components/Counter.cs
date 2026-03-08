using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Counter : MonoBehaviour
{
    public UnityEvent onValueChange = new UnityEvent();

    public Button btnUp;
    public Button btnDown;
    public TMP_InputField inputField;

    public float min;
    public float max;
    public float step;

    public float value { get; private set; }

    private void Awake()
    {
        btnUp.onClick.AddListener(OnUp);
        btnDown.onClick.AddListener(OnDown);

        inputField.onEndEdit.AddListener(OnTextChanged);
    }

    public void SetValue(float value)
    {
        this.value = value;
        UpdateText();

        onValueChange.Invoke();
    }

    public void SetInteractable(bool interactable)
    {
        inputField.interactable = interactable;
        btnUp.interactable = interactable;
        btnDown.interactable = interactable;
    }

    void OnUp()
    {
        value = Mathf.Clamp(value + step, min, max);
        UpdateText();

        onValueChange.Invoke();
    }
    void OnDown()
    {
        value = Mathf.Clamp(value - step, min, max);
        UpdateText();

        onValueChange.Invoke();
    }

    void OnTextChanged(string text)
    {
        if (!float.TryParse(text, out float result))
            return;

        inputField.text = "" + Mathf.Clamp(result, min, max);
        value = float.Parse(inputField.text);
    }

    void UpdateText()
    {
        inputField.text = "" + value;
    }
}
