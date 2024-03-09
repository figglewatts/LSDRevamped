using System;
using LSDR.Dream;
using LSDR.Game;
using UnityEngine;

namespace LSDR.Lua
{
    public class JournalScriptHandler : MonoBehaviour
    {
        public SettingsSystem SettingsSystem;
        public DreamSystem DreamSystem;

        public void Start()
        {
            SettingsSystem.CurrentJournal.CreateScript();
        }

        public void Update()
        {
            SettingsSystem.CurrentJournal.UpdateScript();
            if (DreamSystem.CurrentDream != null) DreamSystem.CurrentDream.UpdateScript();
        }
    }
}
