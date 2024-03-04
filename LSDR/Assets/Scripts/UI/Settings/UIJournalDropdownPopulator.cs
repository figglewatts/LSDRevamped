using System.Linq;
using LSDR.Game;
using Torii.UI;

namespace LSDR.UI.Settings
{
    /// <summary>
    ///     Populate the dream journal dropdown menu.
    /// </summary>
    public class UIJournalDropdownPopulator : DropdownPopulator
    {
        public SettingsSystem SettingsSystem;

        protected override void Awake()
        {
            base.Awake();
            PopulateDropdownWithJournals();
        }

        public void PopulateDropdownWithJournals()
        {
            Populate(SettingsSystem.CurrentMod.Journals.Select(j => j.Name).ToList());
        }
    }
}
