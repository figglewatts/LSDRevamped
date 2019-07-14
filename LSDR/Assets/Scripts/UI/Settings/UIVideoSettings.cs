using UnityEngine;
using Game;
using Torii.Binding;
using Torii.UI;

namespace UI
{
	/// <summary>
	/// Script for video settings.
	/// </summary>
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
            GameSettings.SettingsBindBroker.RegisterData(UseClassicShadersToggle);
            GameSettings.SettingsBindBroker.RegisterData(UsePixelationShaderToggle);
            GameSettings.SettingsBindBroker.RegisterData(FullscreenToggle);
            GameSettings.SettingsBindBroker.RegisterData(LimitFramerateToggle);
            GameSettings.SettingsBindBroker.RegisterData(FOVSlider);
            GameSettings.SettingsBindBroker.RegisterData(ResolutionDropdown);
            GameSettings.SettingsBindBroker.RegisterData(QualityDropdown);

            UseClassicShadersToggle.isOn = GameSettings.CurrentSettings.UseClassicShaders;
            UsePixelationShaderToggle.isOn = GameSettings.CurrentSettings.UsePixelationShader;
            FullscreenToggle.isOn = GameSettings.CurrentSettings.Fullscreen;
            LimitFramerateToggle.isOn = GameSettings.CurrentSettings.LimitFramerate;
            FOVSlider.value = GameSettings.CurrentSettings.FOV;
            ResolutionDropdown.value = GameSettings.CurrentSettings.CurrentResolutionIndex;
            QualityDropdown.value = GameSettings.CurrentSettings.CurrentQualityIndex;

            GameSettings.SettingsBindBroker.Bind(() => UseClassicShadersToggle.isOn,
		        () => GameSettings.CurrentSettings.UseClassicShaders, BindingType.TwoWay);
		    GameSettings.SettingsBindBroker.Bind(() => UsePixelationShaderToggle.isOn,
		        () => GameSettings.CurrentSettings.UsePixelationShader, BindingType.TwoWay);
		    GameSettings.SettingsBindBroker.Bind(() => FullscreenToggle.isOn, 
		        () => GameSettings.CurrentSettings.Fullscreen, BindingType.TwoWay);
		    GameSettings.SettingsBindBroker.Bind(() => FOVSlider.value, () => GameSettings.CurrentSettings.FOV,
		        BindingType.TwoWay);
		    GameSettings.SettingsBindBroker.Bind(() => ResolutionDropdown.value,
		        () => GameSettings.CurrentSettings.CurrentResolutionIndex, BindingType.TwoWay);
		    GameSettings.SettingsBindBroker.Bind(() => QualityDropdown.value,
		        () => GameSettings.CurrentSettings.CurrentQualityIndex, BindingType.TwoWay);
		    GameSettings.SettingsBindBroker.Bind(() => LimitFramerateToggle.isOn,
		        () => GameSettings.CurrentSettings.LimitFramerate, BindingType.TwoWay);
		}
	}
}