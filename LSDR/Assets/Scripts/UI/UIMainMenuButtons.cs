using UnityEngine;
using System.Collections;
using Entities.Dream;

namespace UI
{
	public class UIMainMenuButtons : MonoBehaviour
	{
		public UIMainMenu MainMenu;
	
		public void StartButtonPressed()
		{
			DreamDirector.BeginDream();
		}

		public void GraphButtonPressed()
		{
			Fader.FadeIn(Color.black, 0.5F, () =>
			{
				MainMenu.ChangeMenuState(UIMainMenu.MenuState.GRAPH);
				Fader.FadeOut(0.5F);
			});
		}

		public void SettingsButtonPressed()
		{
			Fader.FadeIn(Color.black, 0.5F, () =>
			{
				MainMenu.ChangeMenuState(UIMainMenu.MenuState.SETTINGS);
				Fader.FadeOut(0.5F);
			});
		}

		public void ExitButtonPressed()
		{
			// TODO: do any actons before exiting
			Application.Quit();
		}
	}
}