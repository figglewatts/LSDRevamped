using UnityEngine;
using System.Collections;
using Game;
using UnityEngine.UI;

namespace UI
{
	public class UIContentSettings : MonoBehaviour
	{
		public Toggle EnableHeadBobToggle;
		public Toggle EnableFootstepSoundsToggle;
		public Dropdown CurrentJournalDropdown;

		public void OnEnable()
		{
			EnableHeadBobToggle.isOn = GameSettings.CurrentSettings.HeadBobEnabled;
			EnableFootstepSoundsToggle.isOn = GameSettings.CurrentSettings.EnableFootstepSounds;
			CurrentJournalDropdown.value = GameSettings.CurrentSettings.CurrentJournalIndex;
		}

		public void UpdateHeadbob(bool value) { GameSettings.CurrentSettings.HeadBobEnabled = value; }
		public void UpdateFootstep(bool value) { GameSettings.CurrentSettings.EnableFootstepSounds = value; }
	}
}