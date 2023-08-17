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

        /// <summary>
        ///     The speed at which to rotate the player. Set in editor.
        /// </summary>
        public float RotationSpeed;

        private void FixedUpdate()
        {
            // if we can control the player and we're not currently in FPS control mode
            if (Settings.CanControlPlayer && !ControlScheme.Current.FpsControls)
            {
                // apply a rotation equal to the current move amount
                float rotAmount = ControlScheme.InputActions.Game.Move.ReadValue<Vector2>().x;
                Vector3 transformRotation = transform.rotation.eulerAngles;
                transformRotation.y += rotAmount * RotationSpeed * Time.deltaTime;
                transform.rotation = Quaternion.Euler(transformRotation);
            }
        }
    }
}
