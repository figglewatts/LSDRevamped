using UnityEngine;
using System.Collections;
using UI;

namespace Game
{
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