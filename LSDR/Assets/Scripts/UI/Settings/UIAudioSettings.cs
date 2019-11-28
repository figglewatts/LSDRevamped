using System;
using LSDR.Game;
using Torii.Binding;
using UnityEngine;
using Torii.UI;

namespace LSDR.UI.Settings
{
	/// <summary>
	/// Script for audio settings in LSDR main menu and pause menu.
	/// </summary>
	public class UIAudioSettings : MonoBehaviour
	{
		public SettingsSystem Settings;
		
		/// <summary>
		/// Reference to the music volume slider. Set in inspector.
		/// </summary>
		public Slider MusicVolumeSlider;

		/// <summary>
		/// Reference to the SFX volume slider. Set in inspector.
		/// </summary>
		public Slider SFXVolumeSlider;

		public void Start()
		{
			// register data with the settings bind broker
			Settings.SettingsBindBroker.RegisterData(MusicVolumeSlider);
			Settings.SettingsBindBroker.RegisterData(SFXVolumeSlider);

			// set to initial values from currently loaded settings
			MusicVolumeSlider.value = Settings.Settings.MusicVolume;
			SFXVolumeSlider.value = Settings.Settings.SFXVolume;

			// add value change listeners
			MusicVolumeSlider.onValueChanged.AddListener(Settings.SetMusicVolume);
			SFXVolumeSlider.onValueChanged.AddListener(Settings.SetSFXVolume);

			// bind the data
			Settings.SettingsBindBroker.Bind(() => MusicVolumeSlider.value,
				() => Settings.Settings.MusicVolume, BindingType.TwoWay);
			Settings.SettingsBindBroker.Bind(() => SFXVolumeSlider.value,
				() => Settings.Settings.SFXVolume, BindingType.TwoWay);
		}
	}
}