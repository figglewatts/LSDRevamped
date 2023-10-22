using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Purchasing;
using Debug = UnityEngine.Debug;

namespace LSDR.Util
{
    public class OpenLogDirectoryInputListener : MonoBehaviour
    {
        protected bool _opened = false;

        public void Update()
        {
            if (Keyboard.current == null) return;

            if (!Keyboard.current.ctrlKey.isPressed)
            {
                _opened = false;
                return;
            }

            if (Keyboard.current.f1Key.wasReleasedThisFrame && !_opened)
            {
                _opened = true;
                openDirectory(Application.persistentDataPath);
            }
        }

        protected void openDirectory(string path)
        {
            try
            {
                switch (Application.platform)
                {
                    case RuntimePlatform.WindowsPlayer:
                    case RuntimePlatform.WindowsEditor:
                        Process.Start("explorer.exe", path.Replace('/', '\\'));
                        break;

                    case RuntimePlatform.OSXPlayer:
                    case RuntimePlatform.OSXEditor:
                        Process.Start("open", path);
                        break;

                    case RuntimePlatform.LinuxPlayer:
                    case RuntimePlatform.LinuxEditor:
                        Process.Start("xdg-open", path);
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to open directory: " + e.Message);
            }
        }
    }
}
