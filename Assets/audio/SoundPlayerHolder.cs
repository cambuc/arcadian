using System.Collections.Generic;
using UnityEngine;

public class SoundPlayerHolder : MonoBehaviour
{
    public static SoundPlayerHolder runtime;

    public SoundPlayer uiSelect;
    public SoundPlayer uiClick;

    public SoundPlayer sfxSelect;
    public SoundPlayer sfxClick;

    public SoundPlayer fleshClick;

    private void Awake()
    {
        runtime = this;
    }
}
