using UnityEngine;
using Game;
using Torii.Binding;
using UnityEditor;
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