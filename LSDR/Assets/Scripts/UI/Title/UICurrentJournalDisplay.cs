using LSDR.Game;
using UnityEngine;
using UnityEngine.UI;

namespace LSDR.UI.Title
{
    public class UICurrentJournalDisplay : MonoBehaviour
    {
        public Text JournalDisplayElement;
        public SettingsSystem SettingsSystem;

        public void Start() { JournalDisplayElement.text = SettingsSystem.CurrentJournal.Name; }
        public void OnEnable() { JournalDisplayElement.text = SettingsSystem.CurrentJournal.Name; }
    }
}
