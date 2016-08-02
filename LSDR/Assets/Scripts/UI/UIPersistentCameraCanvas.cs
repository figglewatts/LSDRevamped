using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace UI
{
	/// <summary>
	/// Used to make a camera canvas that persists between scene loads.
	/// Finds the main camera whenever a new scene is loaded.
	/// </summary>
	public class UIPersistentCameraCanvas : MonoBehaviour
	{
		private Canvas _canvas;

		public void Start()
		{
			_canvas = GetComponent<Canvas>();
			SceneManager.sceneLoaded += OnSceneChange;
		}

		private void OnSceneChange(Scene scene, LoadSceneMode loadSceneMode) { _canvas.worldCamera = Camera.main; }
	}
}