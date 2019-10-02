using UnityEngine;
using LSDR.Game;
using UnityEngine.UI;

namespace LSDR.UI.Settings
{
    /// <summary>
    /// Script for the entire settings menu.
    /// </summary>
    public class UISettings : MonoBehaviour
    {
        public void ApplySettings()
        {
            GameSettings.ApplySettings(GameSettings.CurrentSettings);
            GameSettings.SaveSettings(GameSettings.CurrentSettings);
        }
    }
}