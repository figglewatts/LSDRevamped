using UnityEngine;
using System.Collections;
using Game;
using Torii.UI;

namespace UI
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
