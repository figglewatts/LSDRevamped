using System;
using LSDR.Game;
using UnityEngine;
using UnityEngine.UI;

namespace LSDR.UI.Title
{
    public class UISequenceController : MonoBehaviour
    {
        public Button DecrementSequenceButton;
        public Button IncrementSequenceButton;
        public SettingsSystem SettingsSystem;

        public void Start()
        {
            DecrementSequenceButton.onClick.AddListener(onSequenceDecrementPressed);
            IncrementSequenceButton.onClick.AddListener(onSequenceIncrementPressed);
        }

        protected void onSequenceDecrementPressed()
        {
            SettingsSystem.Settings.DecrementJournal();
        }

        protected void onSequenceIncrementPressed()
        {
            SettingsSystem.Settings.IncrementJournal();
        }
    }
}
