using UnityEngine;
using System.Collections;

//Credit to @wtfmig
//https://twitter.com/wtfmig
//https://pastebin.com/5mkBeZ5S

[ExecuteInEditMode]
public class PixelCamera : MonoBehaviour
{
    public int w = 720;
    int h;
    void Update()
    {

        float ratio = ((float)Camera.main.pixelHeight / (float)Camera.main.pixelWidth);
        h = Mathf.RoundToInt(w * ratio);

    }
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        source.filterMode = FilterMode.Point;
        RenderTexture buffer = RenderTexture.GetTemporary(w, h, -1);
        buffer.filterMode = FilterMode.Point;
        Graphics.Blit(source, buffer);
        Graphics.Blit(buffer, destination);
        RenderTexture.ReleaseTemporary(buffer);
    }
}