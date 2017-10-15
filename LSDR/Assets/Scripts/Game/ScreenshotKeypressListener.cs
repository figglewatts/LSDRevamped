using UnityEngine;
using System.Collections;
using System.IO;
using Util;

namespace Game
{
	public class ScreenshotKeypressListener : MonoBehaviour
	{
		private int _screenshotNumber;

		private string _screenshotPath;

		public void Awake()
		{
			_screenshotNumber = PlayerPrefs.GetInt("LSDR.ScreenshotCounter");
			_screenshotPath = IOUtil.PathCombine(Application.persistentDataPath, "screenshots");
		}
	
		void Update()
		{
			if (Input.GetKeyDown(KeyCode.F9))
			{
				TakeScreenshot();
			}
		}

		private void TakeScreenshot()
		{
			if (!Directory.Exists(_screenshotPath)) Directory.CreateDirectory(_screenshotPath);

			Debug.Log("Taking screenshot");
            ScreenCapture.CaptureScreenshot(IOUtil.PathCombine(_screenshotPath, "Screenshot_" + _screenshotNumber + ".png"));
			_screenshotNumber++;
			PlayerPrefs.SetInt("LSDR.ScreenshotCounter", _screenshotNumber);
		}
	}
}