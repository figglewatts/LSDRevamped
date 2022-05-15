using System.Linq;
using LSDR.Game;
using Torii.UI;

namespace LSDR.UI.Settings
{
    /// <summary>
    /// Populate the mod dropdown menu.
    /// </summary>
    public class UIModDropdownPopulator : DropdownPopulator
    {
        public ModLoaderSystem ModLoaderSystem;

        protected override void Awake()
        {
            base.Awake();
            Populate(ModLoaderSystem.Mods.Select(m => m.Name).ToList());
        }
    }
}
