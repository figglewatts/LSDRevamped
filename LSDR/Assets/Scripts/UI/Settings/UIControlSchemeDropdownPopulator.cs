using System.Linq;
using LSDR.InputManagement;
using Torii.UI;

namespace LSDR.UI.Settings
{
	/// <summary>
	/// Populates the control scheme selector dropdown menu.
	/// </summary>
	public class UIControlSchemeDropdownPopulator : DropdownPopulator
	{
		protected override void Awake()
		{
			base.Awake();
			PopulateDropdown();
		}

		public void PopulateDropdown()
		{
			var list = ControlSchemeManager.Schemes.Select(scheme => scheme.Name).ToList();
			list.Sort();
			Populate(list);
		}
	}
}