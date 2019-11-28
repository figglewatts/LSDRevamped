using LSDR.Game;
using LSDR.InputManagement;
using Torii.Binding;
using Torii.UI;
using UnityEngine;

namespace LSDR.UI.Settings
{
    /// <summary>
    /// Menu for setting control scheme settings.
    /// </summary>
    public class UIControlSettings : MonoBehaviour
    {
        public SettingsSystem Settings;
        
        public Dropdown CurrentSchemeDropdown;

        public void Start()
        {
            Settings.SettingsBindBroker.RegisterData(CurrentSchemeDropdown);

            CurrentSchemeDropdown.value = Settings.Settings.CurrentControlSchemeIndex;

            CurrentSchemeDropdown.onValueChanged.AddListener(ControlSchemeManager.UseScheme);

            Settings.SettingsBindBroker.Bind(() => CurrentSchemeDropdown.value,
                () => Settings.Settings.CurrentControlSchemeIndex, BindingType.TwoWay);
        }
    }
}