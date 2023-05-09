using System;
using System.IO;
using LSDR.SDK.Lua;
using Torii.Console;
using Torii.Util;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LSDR.Visual
{
    public class Screenshotter : MonoSingleton<Screenshotter>
    {
        private const string SCREENSHOT_DIR = "screenshots";

        public void Update()
        {
            if (Keyboard.current[Key.F9].wasPressedThisFrame) TakeScreenshot();
        }

        public void Initialise()
        {
            DevConsole.Register(this);
            LuaManager.Managed.RegisterGlobalObject(this);
        }

        [Console]
        public void TakeScreenshot()
        {
            string filename = $"{DateTime.Now:yy-MM-dd-HH-mm-ss}.png";
            string screenshotDir = PathUtil.Combine(Application.persistentDataPath, SCREENSHOT_DIR);
            Directory.CreateDirectory(screenshotDir);
            ScreenCapture.CaptureScreenshot(PathUtil.Combine(Application.persistentDataPath, SCREENSHOT_DIR,
                filename));
            Debug.Log("Captured screenshot");
        }
    }
}
