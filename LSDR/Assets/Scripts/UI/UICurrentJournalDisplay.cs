using LSDR.Game;
using UnityEngine;
using UnityEngine.UI;

namespace LSDR.UI
{
	// TODO: refactor UICurrentJournalDisplay in DreamDirector refactor
	public class UICurrentJournalDisplay : MonoBehaviour
	{
		public Text JournalDisplayElement;

		public void Start() { JournalDisplayElement.text = DreamJournalManager.CurrentJournal; }
		public void OnEnable() { JournalDisplayElement.text = DreamJournalManager.CurrentJournal; }
	}
}
