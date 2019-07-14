using UnityEngine;
using LSDR.UI;

namespace LSDR.Game
{
	// TODO: Refactor DevConsoleKeypressListener in DevConsole refactor
	public class DevConsoleKeypressListener : MonoBehaviour
	{
		public UIDevConsole DevConsoleScript;
	
		void Update()
		{
			// the key to the left of 1 on the keyboard
			if (Input.GetKeyDown(KeyCode.BackQuote)) DevConsoleScript.ToggleConsoleState();
		}
	}
}