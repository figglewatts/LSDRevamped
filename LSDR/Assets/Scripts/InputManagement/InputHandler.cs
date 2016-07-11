using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace InputManagement
{
	public static class InputHandler
	{
		private static readonly List<string> _inputNames = new List<string>();
		private static readonly List<KeyCode> _inputs = new List<KeyCode>();
		private static ButtonState[] _buttonStates;

		public static void AddInput(string buttonName, KeyCode key)
		{
			_inputNames.Add(buttonName);
			_inputs.Add(key);
		}

		private static void RebindInput(string buttonName, KeyCode key)
		{
			_inputs[_inputNames.IndexOf(buttonName)] = key;
		}

		/// <summary>
		/// Must be called after all inputs have been added.
		/// No inputs can be added during gameplay.
		/// </summary>
		public static void Initialize()
		{
			_buttonStates = new ButtonState[_inputs.Count];
		}

		public static void HandleInput()
		{
			for (int i = 0; i < _buttonStates.Length; i++)
			{
				if (Input.GetKeyDown(_inputs[i])) { _buttonStates[i] = ButtonState.DOWN; }
				else if (Input.GetKey(_inputs[i])) { _buttonStates[i] = ButtonState.HELD; }
				else if (Input.GetKeyUp(_inputs[i])) { _buttonStates[i] = ButtonState.UP; }
				else { _buttonStates[i] = ButtonState.IDLE; }
			}
		}

		public static bool CheckButtonState(string buttonName, ButtonState state)
		{
			return _buttonStates[_inputNames.IndexOf(buttonName)] == state;
		}

		public static IEnumerator RebindKey(string buttonName)
		{
			bool rebound = false;
			while (!rebound)
			{
				if (Input.anyKey)
				{
					foreach (KeyCode k in Enum.GetValues(typeof(KeyCode)))
					{
						if (Input.GetKeyDown(k))
						{
							RebindInput(buttonName, k);
							rebound = true;
						}
					}
				}
				yield return null;
			}
		}
	}

	public enum ButtonState
	{
		DOWN,
		HELD,
		UP,
		IDLE
	}
}
