using System.Collections.Generic;
using System.Linq;
using Torii.UI;
using UnityEngine;

namespace LSDR.UI.Settings
{
    /// <summary>
    ///     Populate the quality settings dropdown menu.
    /// </summary>
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
