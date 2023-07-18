using System;
using LSDR.Game;
using LSDR.InputManagement;
using UnityEngine;

namespace LSDR.Entities.Player
{
    public class SettingsProfileSwitcher : MonoBehaviour
    {
        public ControlSchemeLoaderSystem ControlScheme;
        public SettingsSystem SettingsSystem;

        public void Update()
        {
            if (Time.timeScale == 0) return;

            if (ControlScheme.InputActions.Game.NextProfile.WasReleasedThisFrame())
            {
                SettingsSystem.SwitchToNextProfile();
            }
        }
    }
}
