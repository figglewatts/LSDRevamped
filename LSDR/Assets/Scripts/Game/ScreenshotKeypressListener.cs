using System;
using UnityEngine;
using System.IO;
using LSDR.Util;

namespace LSDR.Game
{
	/// <summary>
	/// ScreenshotKeypressListener is a MonoBehaviour that, when put on something, will listen for the screenshot key
	/// to be pressed, and take a screenshot.
	/// </summary>
	public class ScreenshotKeypressListener : MonoBehaviour
	{
		// the path to the directory where screenshots are saved
		private readonly string SCREENSHOT_PATH = IOUtil.PathCombine(Application.persistentDataPath, "screenshots");

		// the key to use to take a screenshot
		private readonly KeyCode SCREENSHOT_KEY = KeyCode.F9;
	
		void Update()
		{
			if (Input.GetKeyDown(SCREENSHOT_KEY))
			{
				TakeScreenshot();
			}
		}

		private void TakeScreenshot()
		{
			// first check if the screenshot directory exists, and create it if it doesn't
			if (!Directory.Exists(SCREENSHOT_PATH)) Directory.CreateDirectory(SCREENSHOT_PATH);

			// put the time in the screenshot name so we don't overwrite
			string screenshotName = "Screenshot_" + DateTime.Now.ToString("MM/dd/yyy HH:mm:ss tt") + ".png";

			// take the screenshot
			Debug.Log("Taking screenshot");
            ScreenCapture.CaptureScreenshot(
	            IOUtil.PathCombine(SCREENSHOT_PATH, screenshotName)
	        );
		}
	}
}