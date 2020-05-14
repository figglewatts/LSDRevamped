using System;
using System.IO;
using Torii.Util;
using UnityEngine;

namespace LSDR.Visual
{
    public class Screenshotter : MonoSingleton<Screenshotter>
    {
        private const string SCREENSHOT_DIR = "screenshots";
        
        public void Update()
        {
            if (Input.GetKeyUp(KeyCode.F9))
            {
                TakeScreenshot();
            }
        }

        public void TakeScreenshot()
        {
            string filename = $"{DateTime.Now:yy-MM-dd-HH-mm-ss}.png";
            var screenshotDir = PathUtil.Combine(Application.persistentDataPath, SCREENSHOT_DIR);
            Directory.CreateDirectory(screenshotDir);
            ScreenCapture.CaptureScreenshot(PathUtil.Combine(Application.persistentDataPath, SCREENSHOT_DIR, filename));
            Debug.Log("Captured screenshot");
        }
    }
}
