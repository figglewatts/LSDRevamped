using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UI
{
	public class UIGraph : MonoBehaviour
	{
		public GameObject MainMenuObject;

		public void GoBackButtonPressed()
		{
			Fader.FadeIn(Color.black, 0.5F, () =>
			{
				gameObject.SetActive(false);
				MainMenuObject.SetActive(true);
				Fader.FadeOut(0.5F);
			});
		}
	}
}
