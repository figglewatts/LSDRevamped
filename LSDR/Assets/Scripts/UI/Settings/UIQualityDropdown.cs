using UnityEngine;
using System.Collections.Generic;
using Torii.UI;

namespace UI
{
	public class UIQualityDropdown : MonoBehaviour
	{
		private Dropdown QualityDropdown;

		// Use this for initialization
		void Start()
		{
			// TODO: set default value when settings are loaded

			QualityDropdown = GetComponent<Dropdown>();
			List<string> qualitySettingNames = new List<string>();
			foreach (string setting in QualitySettings.names)
			{
				qualitySettingNames.Add(setting);
			}
			QualityDropdown.AddOptions(qualitySettingNames);
		}
	}
}