using UnityEngine;
using System.Collections.Generic;
using Torii.UI;

namespace UI
{
	public class UIResolutionDropdown : MonoBehaviour
	{
		private Dropdown ResolutionDropdown;

		// Use this for initialization
		void Start()
		{
			// TODO: set default value when settings are loaded

			ResolutionDropdown = GetComponent<Dropdown>();
			List<string> resolutions = new List<string>();
			foreach (Resolution res in Screen.resolutions)
			{
				resolutions.Add(res.width + "x" + res.height);
			}
			ResolutionDropdown.AddOptions(resolutions);
		}
	}
}