using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenCaptureDemo : MonoBehaviour 
{
	// Use this for initialization
	void Start () {
        StartCoroutine(Caputure());
	}
	
    private IEnumerator Caputure()
    {
        yield return new WaitForEndOfFrame();

        Rect rect = new Rect(0, 0, Screen.width, Screen.height);

        //ScreenCapture.Instance.DefaultCapture("123.png");
        string filePath = Application.dataPath + "/ScreenCapture/123.png";
        Texture2D texture = ScreenCapture.Instance.UseCamTargetTexture(Camera.main,rect);
        //Texture2D resized = ScreenCapture.Instance.ResizeUseMipmap(texture,2); 

        Texture2D resized = ScreenCapture.Instance.ResizeUseResolution(texture,100,200);
        ScreenCapture.Instance.Save2Png(resized, filePath);

    }

    
	// Update is called once per frame
	void Update () 
    {
		
	}
}
