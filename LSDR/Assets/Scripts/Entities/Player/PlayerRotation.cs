using LSDR.Game;
using LSDR.InputManagement;
using UnityEngine;

namespace LSDR.Entities.Player
{
    /// <summary>
    ///     Handles player left and right rotation. This is for classic control mode.
    /// </summary>
    public class PlayerRotation : MonoBehaviour
    {
        public SettingsSystem Settings;
        public ControlSchemeLoaderSystem ControlScheme;
        public PlayerCameraRotation CameraRotation;

        /// <summary>
        ///     The speed at which to rotate the player. Set in editor.
        /// </summary>
        public float RotationSpeed;

        protected bool _externalInput = false;
        protected float _externalInputValue;

        public void UseExternalInput(float inputValue)
        {
            _externalInput = true;
            _externalInputValue = inputValue;
            CameraRotation.SetUsingExternalInput(true);
        }

        public void StopExternalInput()
        {
            _externalInput = false;
            _externalInputValue = 0;
            CameraRotation.SetUsingExternalInput(false);
        }

        private void Update()
        {
            // if we can control the player and we're not currently in FPS control mode (or we're using external input)
            if (Settings.CanControlPlayer && (!ControlScheme.Current.FpsControls || _externalInput))
            {
                // apply a rotation equal to the current move amount
                float rotAmount = ControlScheme.InputActions.Game.Move.ReadValue<Vector2>().x;
                if (_externalInput) rotAmount = _externalInputValue;
                Vector3 transformRotation = transform.rotation.eulerAngles;
                transformRotation.y += rotAmount * RotationSpeed * Time.deltaTime;
                transform.rotation = Quaternion.Euler(transformRotation);
            }
        }
    }
}
