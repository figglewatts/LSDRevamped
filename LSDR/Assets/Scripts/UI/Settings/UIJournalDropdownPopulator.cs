using LSDR.Game;
using Torii.UI;

namespace LSDR.UI.Settings
{
	/// <summary>
	/// Populate the dream journal dropdown menu.
	/// </summary>
	public class UIJournalDropdownPopulator : DropdownPopulator
	{
		protected override void Awake()
		{
			base.Awake();
			Populate(DreamJournalManager.LoadedJournals);
		}
	}
}
