using UnityEngine;
using System;

public class ScreenshotScript : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TakeScreenshot();
        }
    }

    void TakeScreenshot()
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        string path = $"Assets/Screenshots/Screenshot_{timestamp}.png";
        ScreenCapture.CaptureScreenshot(path, 1);
        Debug.Log($"Screenshot saved: {path}");
    }
}