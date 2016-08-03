using UnityEngine;
using Game;
using InputManagement;

namespace Entities.Player
{
	/// <summary>
	/// Handles player left and right rotation. This is for classic control mode.
	/// </summary>
	public class PlayerRotation : MonoBehaviour
	{
		public float RotationSpeed = 1.4F;

		void Update()
		{
			if (GameSettings.CanControlPlayer && !ControlSchemeManager.CurrentScheme.FPSMovementEnabled)
			{
				if (InputHandler.CheckButtonState("Left", ButtonState.HELD))
				{
					Vector3 transformRotation = transform.rotation.eulerAngles;
					transformRotation.y -= RotationSpeed * Time.deltaTime;
					transform.rotation = Quaternion.Euler(transformRotation);
				}
				else if (InputHandler.CheckButtonState("Right", ButtonState.HELD))
				{
					Vector3 transformRotation = transform.rotation.eulerAngles;
					transformRotation.y += RotationSpeed * Time.deltaTime;
					transform.rotation = Quaternion.Euler(transformRotation);
				}
			}
		}
	}
}
