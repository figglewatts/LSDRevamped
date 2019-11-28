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
    public class CameraSettingsBinding : MonoBehaviour, IPropertyWatcher
    {
        /// <summary>
        /// The Camera to bind the FOV setting to. Set in inspector.
        /// </summary>
        public Camera Camera;

        public PixelateImageEffect PixelateImageEffect;

        public SettingsSystem Settings;

        private BindBroker _broker;

        /// <summary>
        /// The FOV of this camera. Used by BindBroker.
        /// </summary>
        public float FOV
        {
            get { return Camera.fieldOfView; }
            set
            {
                Camera.fieldOfView = value;
                NotifyPropertyChange(nameof(FOV));
            }
        }

        /// <summary>
        /// To enable/disable the pixelation effect.
        /// </summary>
        public bool PixelationEffect
        {
            get { return PixelateImageEffect.enabled; }
            set
            {
                PixelateImageEffect.enabled = value;
                NotifyPropertyChange(nameof(PixelationEffect));
            }
        }

        // Use this for initialization
        void Start()
        {
            // create and register the BindBroker
            _broker = new BindBroker();
            _broker.RegisterData(Settings.Settings);
            GUID = Guid.NewGuid();

            // set the FOV and pixelation to the current value from game settings
            FOV = Settings.Settings.FOV;
            PixelationEffect = Settings.Settings.UsePixelationShader;

            // bind the settings FOV and pixelation values to the Camera FOV and image effect, so we get updates
            _broker.Bind(() => Settings.Settings.FOV, () => FOV, BindingType.OneWay);
            _broker.Bind(() => Settings.Settings.UsePixelationShader, () => PixelationEffect, BindingType.OneWay);
        }

        #region BindBroker variables

        public event Action<string, IPropertyWatcher> OnPropertyChange;
        public Guid GUID { get; private set; }
        public void NotifyPropertyChange(string propertyName) { OnPropertyChange?.Invoke(propertyName, this); }

        #endregion
    }
}