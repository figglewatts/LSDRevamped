using UnityEngine;
using System.Collections;
using Entities.Dream;

namespace UI
{
	public class UIMainMenuButtons : MonoBehaviour
	{
		public GameObject GraphObject;
		public GameObject SettingsObject;
	
		public void StartButtonPressed()
		{
			DreamDirector.BeginDream();
		}

		public void GraphButtonPressed()
		{
			Fader.FadeIn(Color.black, 0.5F, () =>
			{
				GraphObject.SetActive(true);
				gameObject.SetActive(false);
				Fader.FadeOut(0.5F);
			});
		}

		public void SettingsButtonPressed()
		{
			Fader.FadeIn(Color.black, 0.5F, () =>
			{
				SettingsObject.SetActive(true);
				gameObject.SetActive(false);
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