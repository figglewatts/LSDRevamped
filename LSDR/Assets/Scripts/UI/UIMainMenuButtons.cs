using UnityEngine;
using LSDR.Entities.Dream;
using Torii.UI;

namespace LSDR.UI
{
	/// <summary>
	/// Script for main menu buttons. Contains their functionality on click.
	/// </summary>
	public class UIMainMenuButtons : MonoBehaviour
	{
		public UIMainMenu MainMenu;
	
		public void StartButtonPressed()
		{
			DreamDirector.BeginDream();
		}

		public void GraphButtonPressed()
		{
			ToriiFader.Instance.FadeIn(Color.black, 0.5F, () =>
			{
				MainMenu.ChangeMenuState(UIMainMenu.MenuState.GRAPH);
				ToriiFader.Instance.FadeOut(0.5F);
			});
		}

		public void SettingsButtonPressed()
		{
			ToriiFader.Instance.FadeIn(Color.black, 0.5F, () =>
			{
				MainMenu.ChangeMenuState(UIMainMenu.MenuState.SETTINGS);
				ToriiFader.Instance.FadeOut(0.5F);
			});
		}

		public void ExitButtonPressed()
		{
			// TODO: do any actons before exiting
			Application.Quit();
		}
	}
}