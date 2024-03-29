﻿using LSDR.Game;
using LSDR.Visual;
using UnityEngine;

namespace LSDR.Bindings
{
    /// <summary>
    ///     MonoBehaviour used to bind the game's FOV setting to the given Camera.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    [RequireComponent(typeof(PixelateImageEffect))]
    public class CameraSettingsBinding : MonoBehaviour
    {
        public PixelateImageEffect PixelateImageEffect;
        public Camera Camera;

        public SettingsSystem Settings;

        public void Start()
        {
            OnSettingsApply();
        }

        public void OnSettingsApply()
        {
            if (Settings.Settings != null)
            {
                if (PixelateImageEffect != null) PixelateImageEffect.enabled = Settings.Settings.UsePixelationShader;
                if (PixelateImageEffect != null) PixelateImageEffect.Dithering = Settings.Settings.UseDithering;
                Camera.fieldOfView = Settings.Settings.FOV;
            }
        }
    }
}
