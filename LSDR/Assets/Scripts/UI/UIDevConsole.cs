using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Entities.Dream;
using Game;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Util;

namespace UI
{
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
		private bool _previousCursorViewState;

		void Start()
		{
			_consoleVisible = gameObject.activeSelf;

			CommandInputField.onEndEdit.AddListener(val =>
			{
				if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) CommandSubmit(val);
			});
		}

		public void ToggleConsoleState() { SetConsoleState(!_consoleVisible); }

		public void SetConsoleState(bool state)
		{
			CommandInputField.text = string.Empty;

			GameSettings.CanMouseLook = !state; // set it so we can't look while dev console is active
			
			_consoleVisible = state;
			gameObject.SetActive(state);

			if (state)
			{
				_previousCursorViewState = Cursor.visible;
				StartCoroutine(UpdateScrollRect());
			}
			else TextFieldSelected(false);

			GameSettings.SetCursorViewState(state ? true : DreamDirector.CurrentlyInDream ? false : true);
		}

		public void ClearConsole()
		{
			foreach (Transform child in ConsoleOutputRowContainer.transform)
			{
				Destroy(child.gameObject);
			}
		}

		// used to make the player not able to move when we're typing
		public void TextFieldSelected(bool selected) { GameSettings.CanControlPlayer = !selected; }

		private void CommandSubmit(string command)
		{
			ProcessConsoleCommand(command);
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

		private void ProcessConsoleCommand(string command)
		{
			List<string> commandFragments = SplitConsoleCommand(command);

			switch (commandFragments[0].ToLowerInvariant())
			{
				case "switchjournal":
				{
					DreamJournalManager.SwitchJournal(commandFragments[1]);
					break;
				}

				case "loadlevel":
				{
					string levelName = commandFragments[1];
					
					// if there is a journal specified switch to it
					if (commandFragments.Count > 2) DreamJournalManager.SwitchJournal(commandFragments[2]);

					string levelPath = IOUtil.PathCombine(Application.dataPath, "levels", DreamJournalManager.CurrentJournal, levelName + ".tmap");

					// check if the level exists before doing anything
					if (!File.Exists(levelPath))
					{
						Debug.LogError("Level " + levelName + " does not exist");
						break;
					}

					// if we're not in a dream begin one with the specified level
					if (!DreamDirector.CurrentlyInDream)
					{
						DreamDirector.BeginDream(levelPath);
						SetConsoleState(false);
						break;
					}
					else
					{
						// otherwise just swap out the level for the specified one
						DreamDirector.SwitchDreamLevel(levelPath);
						break;
					}
                }

				case "textureset":
				{
					int set = int.Parse(commandFragments[1]);
                    Shader.SetGlobalInt("_TextureSet", set);
					Debug.Log("Switched texture set to " + (TextureSet)set);
					break;
				}

				case "enddream":
				{
					DreamDirector.EndDream();
					break;
				}
			
				default:
				{
					Debug.LogWarning("Did not recognize command: " + commandFragments[0]);
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

		private List<string> SplitConsoleCommand(string command)
		{
			return command.Split('"')
					 .Select((element, index) => index % 2 == 0  // If even index
										   ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)  // Split the item
										   : new string[] { element })  // Keep the entire item
					 .SelectMany(element => element).ToList();
		}
	}
}