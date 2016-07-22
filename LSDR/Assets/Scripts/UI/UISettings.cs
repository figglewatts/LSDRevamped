using UnityEngine;
using System.Collections.Generic;
using UI;

public class UISettings : MonoBehaviour
{
	public GameObject MainMenuObject;

	public List<GameObject> MenuTabObjects = new List<GameObject>();

	public void BackButtonPressed()
	{
		Fader.FadeIn(Color.black, 0.5F, () =>
		{
			gameObject.SetActive(false);
			MainMenuObject.SetActive(true);
			Fader.FadeOut(0.5F);
		});
	}

	public void ChangeTab(GameObject o)
	{
		foreach (GameObject obj in MenuTabObjects)
		{
			obj.SetActive(obj == o);
		}
	}
}
