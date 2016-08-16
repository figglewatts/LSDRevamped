using UnityEngine;
using System.Collections;
using Entities.Dream;
using UnityEngine.UI;

namespace UI
{
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
			Debug.Log("Setting day text");
		}
	}
}