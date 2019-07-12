using UnityEngine;
using UI;

namespace Game
{
	/// <summary>
	/// Used to set up the log callback for the developer console
	/// so it can get output from the unity console
	/// </summary>
	// TODO: DevConsole refactor: Refactor DevConsoleLogCallback
	public class DevConsoleLogCallback : MonoBehaviour
	{
		public UIDevConsole DevConsole;

		public void Awake() { Application.logMessageReceived += HandleLog; }

		private void HandleLog(string logString, string stackTrace, LogType type)
		{
			DevConsole.InstantiateOutputRow(logString, type);
			StartCoroutine(DevConsole.UpdateScrollRect());
		}
	}
}