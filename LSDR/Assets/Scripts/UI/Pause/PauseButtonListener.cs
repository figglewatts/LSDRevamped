using LSDR.InputManagement;
using Torii.Event;
using UnityEngine;

namespace LSDR.UI.Pause
{
    public class PauseButtonListener : MonoBehaviour
    {
        public ToriiEvent ToTrigger;
        public ControlSchemeLoaderSystem Controls;

        public void Update()
        {
            if (Controls.InputActions.Game.Pause.WasReleasedThisFrame()) ToTrigger.Raise();
        }
    }
}
