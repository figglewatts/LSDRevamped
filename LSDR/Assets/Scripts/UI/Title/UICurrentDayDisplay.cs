using System;
using LSDR.Game;
using UnityEngine;
using UnityEngine.UI;

namespace LSDR.UI.Title
{
    public class UICurrentDayDisplay : MonoBehaviour
    {
        public Text DayTextElement;
        public GameSaveSystem GameSave;

        public Color TextColA = new Color(r: 0.78f, g: 0.553f, b: 0.635f);
        public Color TextColB = new Color(r: 0.533f, g: 0.498f, b: 0.773f);
        public Color TextColC = new Color(r: 0.431f, g: 0.8f, b: 0.518f);
        public Color TextColD = Color.white;

        public void Start()
        {
            SetDayText(GameSave.CurrentJournalSave.DayNumber);
        }

        public void OnEnable()
        {
            GameSave.CurrentJournalSave.OnDayNumberChanged += onDayNumberChanged;
        }

        public void OnDisable()
        {
            GameSave.CurrentJournalSave.OnDayNumberChanged -= onDayNumberChanged;
        }

        private void onDayNumberChanged()
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
