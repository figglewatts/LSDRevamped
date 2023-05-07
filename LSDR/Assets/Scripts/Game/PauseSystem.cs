using System;
using Torii.Event;
using Torii.Util;
using UnityEngine;

namespace LSDR.Game
{
    [CreateAssetMenu(menuName = "System/PauseSystem")]
    public class PauseSystem : ScriptableObject
    {
        public ToriiEvent OnGamePaused;
        public ToriiEvent OnGameUnpaused;
        public SettingsSystem Settings;
        public bool CanPause = true;
        public bool Paused => Math.Abs(Time.timeScale) < float.Epsilon;

        public void TogglePause()
        {
            if (!CanPause) return;

            if (Paused)
                Unpause();
            else
                Pause();
        }

        public void Pause()
        {
            if (!CanPause) return;

            Time.timeScale = 0;
            OnGamePaused.Raise();
            Settings.CanMouseLook = false;
            ToriiCursor.Show();
        }

        public void Unpause()
        {
            Time.timeScale = 1;
            OnGameUnpaused.Raise();
            Settings.CanMouseLook = true;
            ToriiCursor.Hide();
        }
    }
}
