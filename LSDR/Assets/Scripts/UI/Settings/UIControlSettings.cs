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
        public Dropdown CurrentSchemeDropdown;

        public void Start()
        {
            GameSettings.SettingsBindBroker.RegisterData(CurrentSchemeDropdown);

            CurrentSchemeDropdown.value = GameSettings.CurrentSettings.CurrentControlSchemeIndex;

            CurrentSchemeDropdown.onValueChanged.AddListener(ControlSchemeManager.UseScheme);

            GameSettings.SettingsBindBroker.Bind(() => CurrentSchemeDropdown.value,
                () => GameSettings.CurrentSettings.CurrentControlSchemeIndex, BindingType.TwoWay);
        }
    }
}