using UnityEngine;
using UnityEngine.UI;
using LSDR.Entities.Dream;
using LSDR.Game;

namespace LSDR.UI
{
	public class UICurrentDayDisplay : MonoBehaviour
	{
		public Text DayTextElement;
		public GameSaveSystem GameSave;
		
		public Color TextColA = new Color(0.78f, 0.553f, 0.635f);
		public Color TextColB = new Color(0.533f, 0.498f, 0.773f);
		public Color TextColC = new Color(0.431f, 0.8f, 0.518f);
		public Color TextColD = Color.white;

		public void Start()
		{
			SetDayText(GameSave.CurrentJournalSave.DayNumber);
		}

		public void OnEnable()
		{
			SetDayText(GameSave.CurrentJournalSave.DayNumber);
		}

		private void SetDayText(int dayNumber)
		{
			DayTextElement.text = $"Day {dayNumber:000}";

			int dayNumMod = GameSave.CurrentJournalSave.DayNumber % 41;
			if (dayNumMod <= 10)
			{
				DayTextElement.color = TextColA;
			}
			else if (dayNumMod <= 20)
			{
				DayTextElement.color = TextColB;
			}
			else if (dayNumMod <= 30)
			{
				DayTextElement.color = TextColC;
			}
			else if (dayNumMod <= 40)
			{
				DayTextElement.color = TextColD;
			}
		}
	}
}