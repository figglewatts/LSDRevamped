using Game;
using Torii.Binding;
using UnityEngine;
using UnityEngine.Audio;
using Torii.UI;

public class UIAudioSettings : MonoBehaviour
{
	public Slider MusicVolumeSlider;
	public Slider SFXVolumeSlider;

	public void Start()
	{
        GameSettings.SettingsBindBroker.RegisterData(MusicVolumeSlider);
        GameSettings.SettingsBindBroker.RegisterData(SFXVolumeSlider);

        MusicVolumeSlider.value = GameSettings.CurrentSettings.MusicVolume;
		SFXVolumeSlider.value = GameSettings.CurrentSettings.SFXVolume;
        GameSettings.SetMusicVolume(GameSettings.CurrentSettings.MusicVolume);
        GameSettings.SetSFXVolume(GameSettings.CurrentSettings.SFXVolume);

        MusicVolumeSlider.onValueChanged.AddListener(GameSettings.SetMusicVolume);
        SFXVolumeSlider.onValueChanged.AddListener(GameSettings.SetSFXVolume);

        GameSettings.SettingsBindBroker.Bind(() => MusicVolumeSlider.value,
            () => GameSettings.CurrentSettings.MusicVolume, BindingType.TwoWay);
        GameSettings.SettingsBindBroker.Bind(() => SFXVolumeSlider.value,
            () => GameSettings.CurrentSettings.SFXVolume, BindingType.TwoWay);
    }
}
