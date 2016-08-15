using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UI
{
	public class UIGraph : MonoBehaviour
	{
		public UIMainMenu MainMenu;

		public void GoBackButtonPressed()
		{
			Fader.FadeIn(Color.black, 0.5F, () =>
			{
				MainMenu.ChangeMenuState(UIMainMenu.MenuState.MAIN);
				Fader.FadeOut(0.5F);
			});
		}
	}
}
