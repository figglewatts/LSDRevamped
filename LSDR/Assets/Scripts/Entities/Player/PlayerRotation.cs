using UnityEngine;
using System.Collections;
using Game;
using InputManagement;

namespace Entities.Player
{
	public class PlayerRotation : MonoBehaviour
	{
		public float rotationSpeed = 1.4F;

		void Update()
		{
			if (GameSettings.CanControlPlayer && !GameSettings.FPSMovementEnabled)
			{
				if (InputHandler.CheckButtonState("Left", ButtonState.HELD))
				{
					Vector3 transformRotation = this.transform.rotation.eulerAngles;
					transformRotation.y -= rotationSpeed * Time.deltaTime;
					this.transform.rotation = Quaternion.Euler(transformRotation);
				}
				else if (InputHandler.CheckButtonState("Right", ButtonState.HELD))
				{
					Vector3 transformRotation = this.transform.rotation.eulerAngles;
					transformRotation.y += rotationSpeed * Time.deltaTime;
					this.transform.rotation = Quaternion.Euler(transformRotation);
				}
			}
		}
	}
}
