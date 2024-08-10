using System;
using LSDR.Game;
using Torii.Binding;
using UnityEngine;
using UnityEngine.UI;

namespace LSDR.UI.Title
{
    public class UIYearsDisplay : MonoBehaviour
    {
        public GameObject SingleYearIconPrefab;
        public GameObject MultiYearIconPrefab;
        public GameSaveSystem GameSave;
        public SettingsSystem SettingsSystem;

        public void Start()
        {
            updateView();
        }

        public void OnEnable()
        {
            GameSave.OnGameLoaded += onDayNumberChanged;
            GameSave.OnSaveDataChanged += onDayNumberChanged;
            SettingsSystem.Settings.OnPropertyChange += onSettingsPropertyChanged;
            updateView();
        }

        public void OnDisable()
        {
            GameSave.OnGameLoaded -= onDayNumberChanged;
            GameSave.OnSaveDataChanged -= onDayNumberChanged;
            SettingsSystem.Settings.OnPropertyChange -= onSettingsPropertyChanged;
        }

        protected void onSettingsPropertyChanged(string propertyName, IPropertyWatcher watcher)
        {
            onDayNumberChanged();
        }

        private void onDayNumberChanged()
        {
            updateView();
        }

        protected void updateView()
        {
            var currentJournal = GameSave.CurrentJournalSave;

            clearIcons();
            if (currentJournal.YearNumber > 10)
            {
                createMultiYearIcon(currentJournal.YearNumber);
            }
            else
            {
                for (int i = 0; i < currentJournal.YearNumber; i++) createYearIcon();
            }
        }

        protected void createYearIcon()
        {
            Instantiate(SingleYearIconPrefab, transform);
        }

        protected void createMultiYearIcon(int count)
        {
            var multiYearIcon = Instantiate(MultiYearIconPrefab, transform);
            var text = multiYearIcon.transform.GetChild(0).GetComponent<Text>();
            text.text = $"x{count}";
        }

        protected void clearIcons()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
