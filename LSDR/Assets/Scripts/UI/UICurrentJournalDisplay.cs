using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	// TODO: refactor UICurrentJournalDisplay in DreamDirector refactor
	public class UICurrentJournalDisplay : MonoBehaviour
	{
		public Text JournalDisplayElement;

		public void Start() { JournalDisplayElement.text = DreamJournalManager.CurrentJournal; }
		public void OnEnable() { JournalDisplayElement.text = DreamJournalManager.CurrentJournal; }
	}
}
