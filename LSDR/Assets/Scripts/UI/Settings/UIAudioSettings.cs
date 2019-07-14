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
			GameSettings.SettingsBindBroker.RegisterData(MusicVolumeSlider);
			GameSettings.SettingsBindBroker.RegisterData(SFXVolumeSlider);

			// set to initial values from currently loaded settings
			MusicVolumeSlider.value = GameSettings.CurrentSettings.MusicVolume;
			SFXVolumeSlider.value = GameSettings.CurrentSettings.SFXVolume;
			GameSettings.SetMusicVolume(GameSettings.CurrentSettings.MusicVolume);
			GameSettings.SetSFXVolume(GameSettings.CurrentSettings.SFXVolume);

			// add value change listeners
			MusicVolumeSlider.onValueChanged.AddListener(GameSettings.SetMusicVolume);
			SFXVolumeSlider.onValueChanged.AddListener(GameSettings.SetSFXVolume);

			// bind the data
			GameSettings.SettingsBindBroker.Bind(() => MusicVolumeSlider.value,
				() => GameSettings.CurrentSettings.MusicVolume, BindingType.TwoWay);
			GameSettings.SettingsBindBroker.Bind(() => SFXVolumeSlider.value,
				() => GameSettings.CurrentSettings.SFXVolume, BindingType.TwoWay);
		}
	}
}