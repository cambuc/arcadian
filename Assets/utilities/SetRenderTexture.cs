using System.Collections.Generic;
using UnityEngine;

public class SetRenderTexture : MonoBehaviour
{
    public RenderTexture render;

    private void Start()
    {
        render = new RenderTexture(render.width, render.height, 24);
    }
}
