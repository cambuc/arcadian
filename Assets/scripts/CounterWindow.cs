using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CounterWindow : MonoBehaviour
{
    public static CounterWindow runtime;
    public static UnityEvent<float> windowClosed = new UnityEvent<float>();

    public GameObject root;
    public Counter counter;
    public Button btnConfirm;
    public Button btnCancel;

    private void Awake()
    {
        runtime = this;
    }

    private void Start()
    {
        Close();
    }

    public void OpenWindow(float min, float max, float step, float startValue, UnityAction<float> onClose)
    {
        PlayerMovement.LockMovement(gameObject, true);

        windowClosed.AddListener(onClose);

        counter.min = min;
        counter.max = max;
        counter.step = step;

        counter.SetValue(startValue);

        btnConfirm.onClick.RemoveAllListeners();
        btnConfirm.onClick.AddListener(Confirm);

        btnCancel.onClick.RemoveAllListeners();
        btnCancel.onClick.AddListener(Cancel);

        root.SetActive(true);
    }

    void Close()
    {
        PlayerMovement.LockMovement(gameObject, false);
        root.SetActive(false);
    }

    public void Confirm()
    {
        Close();

        windowClosed.Invoke(counter.value);
        windowClosed.RemoveAllListeners();
    }
    public void Cancel()
    {
        Close();

        windowClosed.Invoke(-1);
        windowClosed.RemoveAllListeners();
    }
}
