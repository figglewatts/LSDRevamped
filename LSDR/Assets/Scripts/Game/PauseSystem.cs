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

        public void TogglePause()
        {
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
