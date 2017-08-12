using UnityEngine;
using System.Collections.Generic;
using Game;
using UI;

public class UISettings : MonoBehaviour
{
	public UIMainMenu MainMenu;

	public List<GameObject> MenuTabObjects = new List<GameObject>();

	public void BackButtonPressed()
	{
		Fader.FadeIn(Color.black, 0.5F, () =>
		{
			MainMenu.ChangeMenuState(UIMainMenu.MenuState.MAIN);
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

	public void ApplySettings()
	{
		GameSettings.SaveSettings();
	    GameSettings.ApplySettings();
        SaveGameManager.LoadGame();
    }
}
