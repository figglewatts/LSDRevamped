using UnityEngine;
using System.Collections;
using Game;
using UnityEngine.UI;

namespace UI
{
	public class UIVideoSettings : MonoBehaviour
	{
		public Toggle UseClassicShadersToggle;
		public Toggle UsePixelationShaderToggle;
		public Toggle FullscreenToggle;
		public Slider FOVSlider;
		public Dropdown ResolutionDropdown;
		public Dropdown QualityDropdown;

		public void OnEnable()
		{
			UseClassicShadersToggle.isOn = GameSettings.UseClassicShaders;
			UsePixelationShaderToggle.isOn = GameSettings.UsePixelationShader;
			FullscreenToggle.isOn = GameSettings.Fullscreen;
			FOVSlider.value = GameSettings.FOV;
			ResolutionDropdown.value = GameSettings.CurrentResolutionIndex;
			QualityDropdown.value = GameSettings.CurrentQualityIndex;
		}

		public void ValueChanged()
		{
			GameSettings.UseClassicShaders = UseClassicShadersToggle.isOn;
			GameSettings.UsePixelationShader = UsePixelationShaderToggle.isOn;
			GameSettings.Fullscreen = FullscreenToggle.isOn;
			GameSettings.FOV = FOVSlider.value;
			GameSettings.CurrentResolutionIndex = ResolutionDropdown.value;
			GameSettings.CurrentQualityIndex = QualityDropdown.value;
		}
	}
}