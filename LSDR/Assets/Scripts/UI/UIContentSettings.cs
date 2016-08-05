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
			EnableHeadBobToggle.isOn = GameSettings.HeadBobEnabled;
			EnableFootstepSoundsToggle.isOn = GameSettings.EnableFootstepSounds;
			CurrentJournalDropdown.value = GameSettings.CurrentJournalIndex;
		}

		public void UpdateHeadbob(bool value) { GameSettings.HeadBobEnabled = value; }
		public void UpdateFootstep(bool value) { GameSettings.EnableFootstepSounds = value; }
	}
}