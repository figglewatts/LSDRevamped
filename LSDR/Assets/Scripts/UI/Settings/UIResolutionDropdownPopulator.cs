using System.Collections.Generic;
using System.Linq;
using Torii.UI;
using UnityEngine;

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
            var resolutions = new List<string>();
            IOrderedEnumerable<Resolution> screenResolutions = Screen
                                                              .resolutions.Distinct().OrderBy(r => r.width)
                                                              .ThenBy(r => r.height)
                                                              .ThenBy(r => r.refreshRate);
            foreach (Resolution res in screenResolutions)
            {
                resolutions.Add($"{res.width}x{res.height} {res.refreshRate}Hz");
            }
            return resolutions;
        }
    }
}
