using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Torii.UI;

namespace UI
{
	public class UIQualityDropdownPopulator : DropdownPopulator
	{
		protected override void Awake()
		{
			base.Awake();
			Populate(getQualityNames());
		}

		private List<string> getQualityNames() { return QualitySettings.names.ToList(); }
	}
}