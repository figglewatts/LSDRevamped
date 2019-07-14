using System;
using UnityEngine;
using System.Collections;
using Entities.Dream;
using Game;
using UnityEngine.UI;

namespace UI
{
	/// <summary>
	/// The pause menu of the game.
	/// TODO: refactor music display in UIPauseMenu
	/// </summary>
	public class UIPauseMenu : MonoBehaviour
	{
		public GameObject PauseScreenBackgroundObject;
		public GameObject PauseMenuObject;
		public GameObject SettingsObject;
		public Text SongNameTextElement;
		public Text SongArtistTextElement;
        public UISettings UISettingsScript;

		private PauseState _pauseState;

		public enum PauseState
		{
			NOT_PAUSED = 0,
			MAIN_PAUSE = 1,
			SETTINGS = 2
		}

		public void Start() { ChangePauseState(PauseState.NOT_PAUSED); } // default to not paused

		public void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7)) // start button
			{
				switch (_pauseState)
				{
					case PauseState.NOT_PAUSED:
					{
						ChangePauseState(PauseState.MAIN_PAUSE);
						break;
					}
					case PauseState.MAIN_PAUSE:
					{
						ChangePauseState(PauseState.NOT_PAUSED);
						break;
					}
					case PauseState.SETTINGS:
					{
						ChangePauseState(PauseState.NOT_PAUSED);
                        UISettingsScript.ApplySettings();
						break;
					}
				}
			}
		}

		public void QuitToMenu()
		{
			DreamDirector.ExitDream();
		}

		public void QuitToDesktop()
		{
			Application.Quit();
		}

		public void ChangePauseState(int state)
		{
			ChangePauseState((PauseState)state);
		}

		public void ChangePauseState(PauseState state)
		{
			_pauseState = state;
			switch (state)
			{
				case PauseState.MAIN_PAUSE:
				{
					UpdateSongDisplayText();
					SettingsObject.SetActive(false);
					PauseMenuObject.SetActive(true);
					PauseScreenBackgroundObject.SetActive(true);
					GameSettings.PauseGame(true);
					break;
				}
				case PauseState.NOT_PAUSED:
				{
					PauseScreenBackgroundObject.SetActive(false);
					PauseMenuObject.SetActive(false);
					SettingsObject.SetActive(false);
					GameSettings.PauseGame(false);
					break;
				}
				case PauseState.SETTINGS:
				{
					SettingsObject.SetActive(true);
					PauseMenuObject.SetActive(false);
					PauseScreenBackgroundObject.SetActive(true);
					break;
				}
            }
		}

		private void UpdateSongDisplayText()
		{
			string songName;
			string artistName;

			if (DreamDirector.CurrentlyPlayingSong.Equals(string.Empty))
			{
				songName = "No song";
				artistName = "No artist";
			}
			else
			{
				string[] songDetails = DreamDirector.CurrentlyPlayingSong.Split(new[] {" - "}, StringSplitOptions.RemoveEmptyEntries);
				songName = songDetails[1];
				artistName = songDetails[0];
			}

			SongNameTextElement.text = songName;
			SongArtistTextElement.text = artistName;
		}
	}
}