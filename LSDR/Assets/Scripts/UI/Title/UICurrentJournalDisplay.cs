using System;
using LSDR.Game;
using UnityEngine;
using UnityEngine.UI;

namespace LSDR.UI.Title
{
    public class UICurrentJournalDisplay : MonoBehaviour
    {
        public Text JournalDisplayElement;
        public SettingsSystem SettingsSystem;

        public void Start()
        {
            JournalDisplayElement.text = SettingsSystem.CurrentJournal.Name;
            SettingsSystem.ProgrammaticOnSettingsApply += updateText;
        }

        public void OnEnable()
        {
            JournalDisplayElement.text = SettingsSystem.CurrentJournal.Name;
        }

        public void OnDestroy()
        {
            SettingsSystem.ProgrammaticOnSettingsApply -= updateText;
        }

        protected void updateText()
        {
            JournalDisplayElement.text = SettingsSystem.CurrentJournal.Name;
        }
    }
}
