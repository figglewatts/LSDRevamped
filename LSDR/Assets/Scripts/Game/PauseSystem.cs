using System;
using Torii.Event;
using UnityEngine;

namespace LSDR.Game
{
    [CreateAssetMenu(menuName="System/PauseSystem")]
    public class PauseSystem : ScriptableObject
    {
        public bool Paused => Math.Abs(Time.timeScale) < float.Epsilon;
        public ToriiEvent OnGamePaused;
        public ToriiEvent OnGameUnpaused;
        public bool CanPause = true;

        public void TogglePause()
        {
            if (!CanPause) return;
            
            if (Paused)
            {
                Unpause();
            }
            else
            {
                Pause();
            }
        }

        public void Pause()
        {
            if (!CanPause) return;
            
            Time.timeScale = 0;
            OnGamePaused.Raise();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public void Unpause()
        {
            Time.timeScale = 1;
            OnGameUnpaused.Raise();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
