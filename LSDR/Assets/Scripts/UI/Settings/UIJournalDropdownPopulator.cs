using System.Linq;
using LSDR.Game;
using Torii.UI;

namespace LSDR.UI.Settings
{
	/// <summary>
	/// Populate the dream journal dropdown menu.
	/// </summary>
	public class UIJournalDropdownPopulator : DropdownPopulator
	{
		public JournalLoaderSystem JournalLoader;
		
		protected override void Awake()
		{
			base.Awake();
			Populate(JournalLoader.Journals.Select(j => j.Name).ToList());
		}
	}
}
