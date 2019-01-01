using UnityEngine;
using System.Collections;
using Game;
using Torii.Binding;
using Torii.UI;

namespace UI
{
	public class UIGameplaySettings : MonoBehaviour
	{
		public Toggle EnableHeadBobToggle;
		public Toggle EnableFootstepSoundsToggle;
		public Dropdown CurrentJournalDropdown;

		public void OnEnable()
		{
            GameSettings.SettingsBindBroker.RegisterData(EnableHeadBobToggle);
            GameSettings.SettingsBindBroker.RegisterData(EnableFootstepSoundsToggle);
            GameSettings.SettingsBindBroker.RegisterData(CurrentJournalDropdown);

            EnableHeadBobToggle.isOn = GameSettings.CurrentSettings.HeadBobEnabled;
			EnableFootstepSoundsToggle.isOn = GameSettings.CurrentSettings.EnableFootstepSounds;
			CurrentJournalDropdown.value = GameSettings.CurrentSettings.CurrentJournalIndex;

            GameSettings.SettingsBindBroker.Bind(() => EnableHeadBobToggle.isOn,
                () => GameSettings.CurrentSettings.HeadBobEnabled, BindingType.TwoWay);
            GameSettings.SettingsBindBroker.Bind(() => EnableFootstepSoundsToggle.isOn,
                () => GameSettings.CurrentSettings.EnableFootstepSounds, BindingType.TwoWay);
            GameSettings.SettingsBindBroker.Bind(() => CurrentJournalDropdown.value,
                () => GameSettings.CurrentSettings.CurrentJournalIndex, BindingType.TwoWay);
        }
	}
}