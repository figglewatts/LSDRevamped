using UnityEngine;
using Game;
using UnityEngine.UI;
using UnityEngine.VR;

namespace UI
{
	public class UIVideoSettings : MonoBehaviour
	{
		public Toggle UseClassicShadersToggle;
		public Toggle UsePixelationShaderToggle;
		public Toggle FullscreenToggle;
		public Toggle LimitFramerateToggle;
		public Slider FOVSlider;
		public Dropdown ResolutionDropdown;
		public Dropdown QualityDropdown;

		public void Start()
		{
			UseClassicShadersToggle.isOn = GameSettings.UseClassicShaders;
			UsePixelationShaderToggle.isOn = GameSettings.UsePixelationShader;
			FullscreenToggle.isOn = GameSettings.Fullscreen;
			FOVSlider.value = GameSettings.FOV;
			ResolutionDropdown.value = GameSettings.CurrentResolutionIndex;
			QualityDropdown.value = GameSettings.CurrentQualityIndex;
			LimitFramerateToggle.isOn = GameSettings.LimitFramerate;
		}

		public void OnEnable()
		{
			UseClassicShadersToggle.isOn = GameSettings.UseClassicShaders;
			UsePixelationShaderToggle.isOn = GameSettings.UsePixelationShader;
			FullscreenToggle.isOn = GameSettings.Fullscreen;
			FOVSlider.value = GameSettings.FOV;
			ResolutionDropdown.value = GameSettings.CurrentResolutionIndex;
			QualityDropdown.value = GameSettings.CurrentQualityIndex;
			LimitFramerateToggle.isOn = GameSettings.LimitFramerate;
		}

		public void ValueChanged()
		{
			GameSettings.FOV = FOVSlider.value;
			GameSettings.CurrentResolutionIndex = ResolutionDropdown.value;
			GameSettings.CurrentQualityIndex = QualityDropdown.value;
		}

		public void ClassicShadersToggleChanged(bool value)
		{
			GameSettings.UseClassicShaders = value && !GameSettings.VR;
		}
		public void PixelationShaderToggleChanged(bool value) { GameSettings.UsePixelationShader = value; }
		public void FullscreenToggleChanged(bool value) { GameSettings.Fullscreen = value; }
		public void FramerateLimitToggleChanged(bool value) { GameSettings.LimitFramerate = value; }
	}
}