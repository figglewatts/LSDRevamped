using System;
using UnityEngine;
using System.Collections;
using LSDR.Entities.Dream;
using LSDR.Game;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LSDR.UI
{
	// TODO: refactor UIDevConsole in DevConsole refactor
	public class UIDevConsole : MonoBehaviour
	{
		public Color LogMessageColor;
		public Color WarningMessageColor;
		public Color ErrorMessageColor;
		public Color ExceptionMessageColor;
		public Sprite LogSprite;
		public Sprite WarningSprite;
		public Sprite ErrorSprite;
		public Sprite ExceptionSprite;

		public GameObject ConsoleOutputRowPrefab;

		public GameObject ConsoleOutputRowContainer;

		public InputField CommandInputField;

		public ScrollRect ContentScrollRect;

		private bool _consoleVisible;

		public void Start()
		{
			_consoleVisible = gameObject.activeSelf;

			CommandInputField.onEndEdit.AddListener(val =>
			{
				if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) CommandSubmit(val);
			});
			
			Application.logMessageReceived += handleLog;
		}

		public void OnDestroy()
		{
			Application.logMessageReceived -= handleLog;
		}

		public void ToggleConsoleState() { SetConsoleState(!_consoleVisible); }

		public void SetConsoleState(bool state)
		{
			CommandInputField.text = string.Empty;

			//GameSettings.CanMouseLook = !state; // set it so we can't look while dev console is active
			
			_consoleVisible = state;
			gameObject.SetActive(state);

			if (state)
			{
				StartCoroutine(UpdateScrollRect());
			}
			else TextFieldSelected(false);

			//GameSettings.SetCursorViewState(state || !DreamDirector.CurrentlyInDream);
		}

		public void ClearConsole()
		{
			foreach (Transform child in ConsoleOutputRowContainer.transform)
			{
				Destroy(child.gameObject);
			}
		}

		// used to make the player not able to move when we're typing
		public void TextFieldSelected(bool selected)
		{
			//GameSettings.CanControlPlayer = !selected;
		}

		private void CommandSubmit(string command)
		{
			DevConsole.ProcessConsoleCommand(command);
			CommandInputField.text = string.Empty;
			EventSystem.current.SetSelectedGameObject(CommandInputField.gameObject);
			CommandInputField.OnPointerClick(new PointerEventData(EventSystem.current));
		}

		public void InstantiateOutputRow(string output, LogType type)
		{
			GameObject outputRow = Instantiate(ConsoleOutputRowPrefab);
			outputRow.transform.SetParent(ConsoleOutputRowContainer.transform, false);

			UIConsoleOutputRow outputRowScript = outputRow.GetComponent<UIConsoleOutputRow>();
			outputRowScript.OutputMessage = output;
			switch (type)
			{
				case LogType.Log:
				{
					outputRowScript.IconColor = LogMessageColor;
					outputRowScript.IconSprite = LogSprite;
					outputRowScript.TextColor = LogMessageColor;
					break;
				}
				case LogType.Warning:
				{
					outputRowScript.IconColor = WarningMessageColor;
					outputRowScript.IconSprite = WarningSprite;
					outputRowScript.TextColor = WarningMessageColor;
					break;
				}
				case LogType.Error:
				{
					outputRowScript.IconColor = ErrorMessageColor;
					outputRowScript.IconSprite = ErrorSprite;
					outputRowScript.TextColor = ErrorMessageColor;
					break;
				}
				case LogType.Assert:
				{
					outputRowScript.IconColor = ErrorMessageColor;
					outputRowScript.IconSprite = ErrorSprite;
					outputRowScript.TextColor = ErrorMessageColor;
					break;
				}
				case LogType.Exception:
				{
					outputRowScript.IconColor = ExceptionMessageColor;
					outputRowScript.IconSprite = ExceptionSprite;
					outputRowScript.TextColor = ExceptionMessageColor;
					break;
				}
			}
		}

		/// <summary>
		/// Used because ScrollRect takes a frame to update, so we need to wait for that
		/// before we set the scrollbar position to the bottom
		/// </summary>
		public IEnumerator UpdateScrollRect()
		{
			yield return new WaitForEndOfFrame();
			ContentScrollRect.verticalNormalizedPosition = 0;
		}
		
		private void handleLog(string logString, string stackTrace, LogType type)
		{
			InstantiateOutputRow(logString, type);
			StartCoroutine(UpdateScrollRect());
		}
	}
}