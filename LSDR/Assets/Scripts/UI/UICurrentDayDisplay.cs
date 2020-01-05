using UnityEngine;
using UnityEngine.UI;
using LSDR.Entities.Dream;
using LSDR.Game;

namespace LSDR.UI
{
	// TODO: refactor UICurrentDayDisplay in DreamDirector refactor
	public class UICurrentDayDisplay : MonoBehaviour
	{
		public Text DayTextElement;
		public GameSaveSystem GameSave;

		public void Start()
		{
			SetDayText(GameSave.CurrentJournalSave.DayNumber);
		}

		public void OnEnable()
		{
			SetDayText(GameSave.CurrentJournalSave.DayNumber);
		}

		private void SetDayText(int dayNumber) { DayTextElement.text = $"Day {dayNumber:000}"; }
	}
}