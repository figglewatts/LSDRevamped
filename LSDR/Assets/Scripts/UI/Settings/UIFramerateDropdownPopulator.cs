using System.Linq;
using LSDR.Game;
using Torii.UI;

namespace LSDR.UI.Settings
{
    // populate the framerate limit dropdown
    public class UIFramerateDropdownPopulator : DropdownPopulator
    {
        protected override void Awake()
        {
            base.Awake();
            Populate(DisplayOptions.FramerateLimits.Select(DisplayOptions.FramerateLabel).ToList());
        }
    }
}