using LSDR.Game;
using UnityEngine;
using UnityEngine.UI;

namespace LSDR.UI.Title
{
	public class UICurrentJournalDisplay : MonoBehaviour
	{
		public Text JournalDisplayElement;
		public JournalLoaderSystem JournalLoader;

		public void Start() { JournalDisplayElement.text = JournalLoader.Current.Name; }
		public void OnEnable() { JournalDisplayElement.text = JournalLoader.Current.Name; }
	}
}
