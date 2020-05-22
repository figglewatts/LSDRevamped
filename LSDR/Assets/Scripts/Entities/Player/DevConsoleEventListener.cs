using LSDR.Game;
using UnityEngine;

namespace LSDR.Entities.Player
{
    public class DevConsoleEventListener : MonoBehaviour
    {
        public PauseSystem PauseSystem;
        public SettingsSystem SettingsSystem;
        
        public void OnDevConsoleOpen()
        {
            SettingsSystem.CanControlPlayer = false;
            SettingsSystem.CanMouseLook = false;
        }

        public void OnDevConsoleClose()
        {
            if (!PauseSystem.Paused)
            {
                SettingsSystem.CanControlPlayer = true;
                SettingsSystem.CanMouseLook = true;
            }
        }
    }
}
