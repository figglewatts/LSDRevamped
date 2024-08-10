using System;
using LSDR.Dream;
using LSDR.Game;
using LSDR.SDK.Data;
using UnityEngine;

namespace LSDR.Lua
{
    public class JournalScriptHandler : MonoBehaviour
    {
        public SettingsSystem SettingsSystem;
        public DreamSystem DreamSystem;

        protected DreamJournal _currentJournal;

        public void Start()
        {
            _currentJournal = SettingsSystem.CurrentJournal;
            _currentJournal.CreateScript();
        }

        public void Update()
        {
            _currentJournal.UpdateScript();
            if (DreamSystem.CurrentDream != null) DreamSystem.CurrentDream.UpdateScript();
        }
    }
}
