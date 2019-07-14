using UnityEngine;
using UnityEngine.UI;
using LSDR.Entities.Dream;

namespace LSDR.UI
{
	// TODO: refactor UICurrentDayDisplay in DreamDirector refactor
	public class UICurrentDayDisplay : MonoBehaviour
	{
		public Text DayTextElement;

		public void Start()
		{
			SetDayText(DreamDirector.CurrentDay);
		}

		public void OnEnable()
		{
			SetDayText(DreamDirector.CurrentDay);
		}

		private void SetDayText(int dayNumber)
		{
			DayTextElement.text = "Day " + dayNumber;
		}
	}
}