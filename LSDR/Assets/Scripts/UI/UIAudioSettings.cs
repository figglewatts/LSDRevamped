using Game;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UIAudioSettings : MonoBehaviour
{
	public Slider MusicVolumeSlider;
	public Slider SFXVolumeSlider;

	public AudioMixer MasterMixer;

	public float baseVal = -80F;
	public float maxVal = 0F;

	public void OnEnable()
	{
		MusicVolumeSlider.value = GameSettings.MusicVolume;
		SFXVolumeSlider.value = GameSettings.SFXVolume;
	}

	public void MusicVolumeChanged(float val)
	{
		GameSettings.MusicVolume = val;
		float dbVolume = baseVal + (val*(maxVal - baseVal));
		MasterMixer.SetFloat("MusicVolume", dbVolume);
	}

	public void SFXVolumeChanged(float val)
	{
		GameSettings.SFXVolume = val;
		float dbVolume = baseVal + (val*(maxVal - baseVal));
		MasterMixer.SetFloat("SFXVolume", dbVolume);
	}
}
