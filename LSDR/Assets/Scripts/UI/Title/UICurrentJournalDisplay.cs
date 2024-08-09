using System;
using LSDR.Game;
using Torii.Binding;
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
            SettingsSystem.Settings.OnPropertyChange += onSettingsPropertyChanged;
        }

        public void OnEnable()
        {
            JournalDisplayElement.text = SettingsSystem.CurrentJournal.Name;
        }

        public void OnDestroy()
        {
            SettingsSystem.ProgrammaticOnSettingsApply -= updateText;
            SettingsSystem.Settings.OnPropertyChange -= onSettingsPropertyChanged;
        }

        protected void onSettingsPropertyChanged(string propertyName, IPropertyWatcher watcher)
        {
            updateText();
        }

        protected void updateText()
        {
            JournalDisplayElement.text = SettingsSystem.CurrentJournal.Name;
        }
    }
}
