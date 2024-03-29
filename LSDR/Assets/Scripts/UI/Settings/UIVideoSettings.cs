﻿using LSDR.Game;
using Torii.Binding;
using Torii.UI;
using UnityEngine;

namespace LSDR.UI.Settings
{
    /// <summary>
    ///     Script for video settings.
    /// </summary>
    public class UIVideoSettings : MonoBehaviour
    {
        public SettingsSystem Settings;

        public Toggle UseClassicShadersToggle;
        public Toggle UsePixelationShaderToggle;
        public Toggle UseDitheringToggle;
        public Toggle FullscreenToggle;
        public Toggle LimitFramerateToggle;
        public Toggle DrawDistanceToggle;
        public Slider FOVSlider;
        public Slider AffineSlider;
        public Dropdown ResolutionDropdown;
        public Dropdown QualityDropdown;

        public void Start()
        {
            Settings.SettingsBindBroker.RegisterData(UseClassicShadersToggle);
            Settings.SettingsBindBroker.RegisterData(UsePixelationShaderToggle);
            Settings.SettingsBindBroker.RegisterData(UseDitheringToggle);
            Settings.SettingsBindBroker.RegisterData(FullscreenToggle);
            Settings.SettingsBindBroker.RegisterData(LimitFramerateToggle);
            Settings.SettingsBindBroker.RegisterData(FOVSlider);
            Settings.SettingsBindBroker.RegisterData(ResolutionDropdown);
            Settings.SettingsBindBroker.RegisterData(QualityDropdown);
            Settings.SettingsBindBroker.RegisterData(AffineSlider);
            Settings.SettingsBindBroker.RegisterData(DrawDistanceToggle);

            if (Application.platform == RuntimePlatform.OSXPlayer)
            {
                UseDitheringToggle.gameObject.transform.parent.gameObject.SetActive(false);
            }

            UseClassicShadersToggle.isOn = Settings.Settings.UseClassicShaders;
            UsePixelationShaderToggle.isOn = Settings.Settings.UsePixelationShader;
            UseDitheringToggle.isOn = Settings.Settings.UseDithering;
            FullscreenToggle.isOn = Settings.Settings.Fullscreen;
            LimitFramerateToggle.isOn = Settings.Settings.LimitFramerate;
            FOVSlider.value = Settings.Settings.FOV;
            ResolutionDropdown.value = Settings.Settings.CurrentResolutionIndex;
            QualityDropdown.value = Settings.Settings.CurrentQualityIndex;
            AffineSlider.value = Settings.Settings.AffineIntensity;
            DrawDistanceToggle.isOn = Settings.Settings.LongDrawDistance;

            Settings.SettingsBindBroker.Bind(() => UseClassicShadersToggle.isOn,
                () => Settings.Settings.UseClassicShaders, BindingType.TwoWay);
            Settings.SettingsBindBroker.Bind(() => UsePixelationShaderToggle.isOn,
                () => Settings.Settings.UsePixelationShader, BindingType.TwoWay);
            Settings.SettingsBindBroker.Bind(() => UseDitheringToggle.isOn, () => Settings.Settings.UseDithering,
                BindingType.TwoWay);
            Settings.SettingsBindBroker.Bind(() => FullscreenToggle.isOn,
                () => Settings.Settings.Fullscreen, BindingType.TwoWay);
            Settings.SettingsBindBroker.Bind(() => FOVSlider.value,
                () => Settings.Settings.FOV,
                BindingType.TwoWay);
            Settings.SettingsBindBroker.Bind(() => ResolutionDropdown.value,
                () => Settings.Settings.CurrentResolutionIndex, BindingType.TwoWay);
            Settings.SettingsBindBroker.Bind(() => QualityDropdown.value,
                () => Settings.Settings.CurrentQualityIndex, BindingType.TwoWay);
            Settings.SettingsBindBroker.Bind(() => LimitFramerateToggle.isOn,
                () => Settings.Settings.LimitFramerate, BindingType.TwoWay);
            Settings.SettingsBindBroker.Bind(() => AffineSlider.value,
                () => Settings.Settings.AffineIntensity,
                BindingType.TwoWay);
            Settings.SettingsBindBroker.Bind(() => DrawDistanceToggle.isOn, () => Settings.Settings.LongDrawDistance,
                BindingType.TwoWay);

            Settings.Settings.ApplyCurrentProfile();
        }
    }
}
