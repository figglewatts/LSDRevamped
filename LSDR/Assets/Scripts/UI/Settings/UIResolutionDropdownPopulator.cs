using System.Collections.Generic;
using System.Linq;
using LSDR.Game;
using Torii.UI;

namespace LSDR.UI.Settings
{
    /// <summary>
    ///     Populate the resolution dropdown menu.
    /// </summary>
    public class UIResolutionDropdownPopulator : DropdownPopulator
    {
        protected override void Awake()
        {
            base.Awake();
            Populate(getResolutions());
        }

        private List<string> getResolutions()
        {
            return DisplayOptions.Resolutions.Select(DisplayOptions.ResolutionLabel).ToList();
        }
    }
}
