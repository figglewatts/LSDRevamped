﻿using UnityEngine;
using System.Collections.Generic;
using Game;
using UI;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Script for the entire settings menu.
/// </summary>
public class UISettings : MonoBehaviour
{
    public Button BackButton;

    public Button.ButtonClickedEvent OnBackButtonPressed;

    public void Start()
    {
        BackButton.onClick = OnBackButtonPressed;
    }

	public void ApplySettings()
	{
	    GameSettings.ApplySettings(GameSettings.CurrentSettings);
        GameSettings.SaveSettings(GameSettings.CurrentSettings);
    }
}