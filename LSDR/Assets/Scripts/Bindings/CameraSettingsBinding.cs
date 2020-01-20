using System;
using LSDR.Game;
using LSDR.Visual;
using Torii.Binding;
using UnityEngine;

namespace LSDR.Bindings
{
    /// <summary>
    /// MonoBehaviour used to bind the game's FOV setting to the given Camera.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    [RequireComponent(typeof(PixelateImageEffect))]
    public class CameraSettingsBinding : MonoBehaviour
    {
        public PixelateImageEffect PixelateImageEffect;
        public Camera Camera;

        public SettingsSystem Settings;

        public void OnSettingsApply()
        {
            PixelateImageEffect.enabled = Settings.Settings.UsePixelationShader;
            Camera.fieldOfView = Settings.Settings.FOV;
        }
    }
}