using LSDR.InputManagement;
using Torii.Event;
using UnityEngine;

namespace LSDR.UI.Pause
{
    public class PauseButtonListener : MonoBehaviour
    {
        public ToriiEvent ToTrigger;
        public ControlSchemeLoaderSystem Controls;

        private int _pressedFrame = -1;

        public void Update()
        {
            if (Controls.InputActions.Game.Pause.WasPressedThisFrame()) _pressedFrame = Time.frameCount;

            // escape is cancel too, so if this press already went back (like resuming) don't pause again
            if (Controls.InputActions.Game.Pause.WasReleasedThisFrame() && UINavigationManager.LastBackFrame < _pressedFrame)
            {
                ToTrigger.Raise();
            }
        }
    }
}
