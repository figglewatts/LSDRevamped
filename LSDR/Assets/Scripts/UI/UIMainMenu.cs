using Torii.UI;
using UnityEngine;

namespace LSDR.UI
{
	/// <summary>
	/// The main menu of the game.
	/// </summary>
	public class UIMainMenu : MonoBehaviour
	{
		public enum MenuState
		{
			MAIN,
			SETTINGS,
			GRAPH
		}

		public MenuState CurrentState;

		[SerializeField]
		private GameObject _mainMenuObject;
		[SerializeField]
		private GameObject _settingsMenuObject;
		[SerializeField]
		private GameObject _graphMenuObject;

		public void ChangeMenuState(int state) { ChangeMenuState((MenuState) state); }

		/// <summary>
		/// Change the menu state, setting game objects and playing animations.
		/// </summary>
		/// <param name="state">The state to change to.</param>
		public void ChangeMenuState(MenuState state)
		{
			switch (state)
			{
				case MenuState.MAIN:
				{
					_mainMenuObject.SetActive(true);
					_settingsMenuObject.SetActive(false);
					_graphMenuObject.SetActive(false);
					break;
				}
				case MenuState.SETTINGS:
				{
					_mainMenuObject.SetActive(false);
					_settingsMenuObject.SetActive(true);
					_graphMenuObject.SetActive(false);
					break;
				}
				case MenuState.GRAPH:
				{
					_mainMenuObject.SetActive(false);
					_settingsMenuObject.SetActive(false);
					_graphMenuObject.SetActive(true);
					break;
				}
            }
			CurrentState = state;
		}

        public void ShowAfterFade()
        {
	        ToriiFader.Instance.FadeIn(Color.black, 0.5F, () =>
            {
                ChangeMenuState(MenuState.MAIN);
                ToriiFader.Instance.FadeOut(0.5F);
            });
        }
	}
}