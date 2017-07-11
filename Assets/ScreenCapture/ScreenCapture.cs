using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class ScreenCapture
{
    private static ScreenCapture _instance = new ScreenCapture();
    public static ScreenCapture Instance { get { return _instance; } }

    static ScreenCapture() { }
    private ScreenCapture() { }


    //stores in project folder
    public void DefaultCapture(string fileName)
    {
        Application.CaptureScreenshot(fileName, 1); 
    }

    //1080 *1920 的RGBA 32 大小只有262KB。   tex.EncodeToPNG() 进行了压缩
    public Texture2D ReadScreenPixels(Rect rect)
    {
        Texture2D tex = new Texture2D(Mathf.CeilToInt(rect.width), Mathf.CeilToInt(rect.height), 
            TextureFormat.ARGB32, true);
        tex.ReadPixels(rect,0,0);
        tex.Apply();
        return tex;
    }

    public Texture2D UseCamTargetTexture(Camera cam,Rect rect)
    {
        RenderTexture rt = new RenderTexture(Mathf.FloorToInt(rect.width),Mathf.FloorToInt(rect.height),24);
        cam.targetTexture = rt;
        cam.Render();

        //IMP
        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(Mathf.FloorToInt(rect.width), Mathf.FloorToInt(rect.height),TextureFormat.ARGB32,true);
        tex.ReadPixels(rect,0,0);
        tex.Apply();

        cam.targetTexture = null;
        RenderTexture.active = null;
        GameObject.Destroy(rt);
        return tex;
    }


    public void Save2Png(Texture2D source,string filePath)
    {
        byte[] pgnFile = source.EncodeToPNG(); //This function works only on ARGB32 and RGB24 texture formats
        GameObject.Destroy(source);
        System.IO.File.WriteAllBytes(filePath, pgnFile);
    }

    //tex.Resize() After resizing, texture pixels will be undefined
    //第一种方案 使用MipMap并且取 Mipmap中的图像  所以只能是1/2 或者 1/4 。。
    //source image must hava mipmap enabled
    public Texture2D ResizeUseMipmap(Texture2D source,int useMipMapLevel)
    {
        if(source.mipmapCount < useMipMapLevel)
        {
            Debug.LogError("source image does not hava mipMapleve " + useMipMapLevel);
        }
        int width = Mathf.FloorToInt(source.width / Mathf.Pow(2,useMipMapLevel));
        int height = Mathf.FloorToInt(source.height / Mathf.Pow(2, useMipMapLevel));
        Texture2D texNew = new Texture2D(width,height,TextureFormat.ARGB32, false);
        texNew.SetPixels(source.GetPixels(useMipMapLevel)); //Texture2D.GetPixels(int miplevel = 0) 
        texNew.Apply();
        return texNew;
    }

    //Resize to any resolution
    public Texture2D ResizeUseResolution(Texture2D source,int targetWidth ,int targetHeight)
    {
        //转换成任意分辨率
        Texture2D texNew = new Texture2D(targetWidth, targetHeight, TextureFormat.ARGB32, false);
        Color[] pixels = texNew.GetPixels(0);
        float incX = (1.0f / targetWidth);
        float incY = (1.0f / targetHeight);
        for(int i = 0;i!= pixels.Length; ++i)
        {
            pixels[i] = source.GetPixelBilinear(i%targetWidth * incX, Mathf.Floor(i/targetWidth) * incY);
        }
        texNew.SetPixels(pixels, 0);
        texNew.Apply();
        return texNew;
    }
    
}
