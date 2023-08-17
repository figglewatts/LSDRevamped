using System.Collections.Generic;
using System.Linq;
using LSDR.InputManagement;
using Torii.UI;

namespace LSDR.UI.Settings
{
    /// <summary>
    ///     Populates the control scheme selector dropdown menu.
    /// </summary>
    public class UIControlSchemeDropdownPopulator : DropdownPopulator
    {
        public ControlSchemeLoaderSystem ControlSchemeLoader;

        protected override void Awake()
        {
            base.Awake();
            PopulateDropdown();
        }

        public void PopulateDropdown()
        {
            List<string> list = ControlSchemeLoader.Schemes.Select(scheme => scheme.Name).ToList();
            list.Sort();
            Populate(list);
        }
    }
}
