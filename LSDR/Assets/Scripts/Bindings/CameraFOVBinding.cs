using System;
using LSDR.Game;
using Torii.Binding;
using UnityEngine;

namespace LSDR.Bindings
{
    /// <summary>
    /// MonoBehaviour used to bind the game's FOV setting to the given Camera.
    /// </summary>
    public class CameraFOVBinding : MonoBehaviour, IPropertyWatcher
    {
        /// <summary>
        /// The Camera to bind the FOV setting to. Set in inspector.
        /// </summary>
        public Camera Camera;

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

        // Use this for initialization
        void Start()
        {
            // create and register the BindBroker
            _broker = new BindBroker();
            _broker.RegisterData(GameSettings.CurrentSettings);
            GUID = Guid.NewGuid();

            // set the FOV to the current value from game settings
            FOV = GameSettings.CurrentSettings.FOV;

            // bind the settings FOV value to the Camera FOV, so we get updates
            _broker.Bind(() => GameSettings.CurrentSettings.FOV, () => FOV, BindingType.OneWay);
        }

        #region BindBroker variables

        public event Action<string, IPropertyWatcher> OnPropertyChange;
        public Guid GUID { get; private set; }
        public void NotifyPropertyChange(string propertyName) { OnPropertyChange?.Invoke(propertyName, this); }

        #endregion
    }
}