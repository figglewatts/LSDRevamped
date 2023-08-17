using LSDR.Game;
using Torii.Binding;
using Torii.UI;
using UnityEngine;

namespace LSDR.UI.Settings
{
    /// <summary>
    ///     Menu for setting gameplay settings.
    /// </summary>
    public class UIGameplaySettings : MonoBehaviour
    {
        public SettingsSystem Settings;

        public Toggle EnableHeadBobToggle;
        public Toggle EnableFootstepSoundsToggle;
        public Dropdown CurrentJournalDropdown;
        public Dropdown CurrentModDropdown;

        public void Start()
        {
            Settings.SettingsBindBroker.RegisterData(EnableHeadBobToggle);
            Settings.SettingsBindBroker.RegisterData(EnableFootstepSoundsToggle);
            Settings.SettingsBindBroker.RegisterData(CurrentJournalDropdown);
            Settings.SettingsBindBroker.RegisterData(CurrentModDropdown);

            EnableHeadBobToggle.isOn = Settings.Settings.HeadBobEnabled;
            EnableFootstepSoundsToggle.isOn = Settings.Settings.EnableFootstepSounds;
            CurrentJournalDropdown.value = Settings.Settings.CurrentJournalIndex;
            CurrentModDropdown.value = Settings.Settings.CurrentModIndex;

            Settings.SettingsBindBroker.Bind(() => EnableHeadBobToggle.isOn,
                () => Settings.Settings.HeadBobEnabled, BindingType.TwoWay);
            Settings.SettingsBindBroker.Bind(() => EnableFootstepSoundsToggle.isOn,
                () => Settings.Settings.EnableFootstepSounds, BindingType.TwoWay);
            Settings.SettingsBindBroker.Bind(() => CurrentJournalDropdown.value,
                () => Settings.Settings.CurrentJournalIndex, BindingType.TwoWay);
            Settings.SettingsBindBroker.Bind(() => CurrentModDropdown.value, () => Settings.Settings.CurrentModIndex,
                BindingType.TwoWay);
        }
    }
}
