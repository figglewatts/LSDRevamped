using UnityEngine;
using System.Collections.Generic;
using Game;
using InputManagement;
using UnityEngine.UI;

namespace UI
{
	public class UIControlSchemeDropdown : MonoBehaviour
	{
		public Dropdown ControlSchemeDropdown;

		// Use this for initialization
		void Start()
		{
			UpdateDropdown();
			ControlSchemeDropdown.value = GameSettings.CurrentControlSchemeIndex;
		}

		void OnEnable()
		{
			UpdateDropdown();
			ControlSchemeDropdown.value = GameSettings.CurrentControlSchemeIndex;
		}

		public void UpdateDropdown()
		{
			ControlSchemeDropdown.ClearOptions();
			List<string> dropdownOptions = new List<string>();
			foreach (ControlScheme scheme in ControlSchemeManager.LoadedSchemes)
			{
				dropdownOptions.Add(scheme.SchemeName);
			}
			ControlSchemeDropdown.AddOptions(dropdownOptions);
		}
	}
}