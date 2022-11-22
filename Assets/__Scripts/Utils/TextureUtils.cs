using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureUtils
{
    public static Texture2D GetTexture2D(RenderTexture renderTexture)
    {
        Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA64, false);
        RenderTexture.active = renderTexture;
        tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        tex.Apply();
        return tex;
    }
}
