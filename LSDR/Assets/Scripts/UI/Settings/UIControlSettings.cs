using LSDR.Game;
using LSDR.InputManagement;
using Torii.Binding;
using Torii.UI;
using UnityEngine;

namespace LSDR.UI.Settings
{
    /// <summary>
    ///     Menu for setting control scheme settings.
    /// </summary>
    public class UIControlSettings : MonoBehaviour
    {
        public SettingsSystem Settings;
        public ControlSchemeLoaderSystem ControlSchemeLoader;

        public Dropdown CurrentSchemeDropdown;
        public Slider LookSensitivitySlider;

        public void Start()
        {
            Settings.SettingsBindBroker.RegisterData(CurrentSchemeDropdown);
            Settings.SettingsBindBroker.RegisterData(LookSensitivitySlider);

            CurrentSchemeDropdown.value = Settings.Settings.CurrentControlSchemeIndex;
            LookSensitivitySlider.value = Settings.Settings.LookSensitivity;

            Settings.SettingsBindBroker.Bind(() => CurrentSchemeDropdown.value,
                () => Settings.Settings.CurrentControlSchemeIndex, BindingType.TwoWay);
            Settings.SettingsBindBroker.Bind(() => LookSensitivitySlider.value,
                () => Settings.Settings.LookSensitivity, BindingType.TwoWay);
        }
    }
}
