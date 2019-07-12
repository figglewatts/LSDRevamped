using UnityEngine;
using Game;
using InputManagement;
using UnityEngine.VR;
using System.Collections.Generic;

namespace Entities.Player
{
	/// <summary>
	/// Controls player camera motion.
	/// Rotates the camera up and down on inputs, and handles mouse look when FPS movement mode is enabled.
	/// </summary>
	public class PlayerCameraRotation : MonoBehaviour
	{
		/// <summary>
		/// The speed at which to rotate the camera. Set in inspector.
		/// </summary>
		public float RotationSpeed;
		
		/// <summary>
		/// The angle at which the player can no longer look up any further. Set in inspector.
		/// </summary>
		public float MaxPositiveRotation;
		
		/// <summary>
		/// The angle at which the player can no longer look down any futher. Set in inspector.
		/// </summary>
		public float MaxNegativeRotation;
		
		/// <summary>
		/// The list of cameras that this should operate upon. Set in inspector.
		/// </summary>
		public List<Camera> TargetCameras;
		
		/// <summary>
		/// If using mouse look deltaTime makes things very slow -- so rotations will be multiplied by this factor.
		/// Set in inspector.
		/// </summary>
		public float MouseLookRotationMultiplier;
		
		/// <summary>
		/// Max Y rotation when using mouse look. Set in inspector.
		/// </summary>
		public float MaxY = 70F;
		
		/// <summary>
		/// Min Y rotation when using mouse look. Set in inspector.
		/// </summary>
		public float MinY = -70F;

		// used to store how much we need to rotate on X axis (for lerping)
		private float _rotationX;

		void Update()
		{
			// if we can't control the player, or we're in VR, then we don't want to affect camera rotation
			if (!GameSettings.CanControlPlayer || GameSettings.VR) return;

			foreach (Camera c in TargetCameras)
			{
				// handle the controls differently depending on whether or not the current
				// control scheme has FPS controls (mouselook) enabled
				if (ControlSchemeManager.Current.FpsControls)
				{
					handleFpsCameraRotation(c);
				}
				else
				{
					handleTankCameraRotation(c);
				}
			}
		}

		/// <summary>
		/// Handle camera rotation as in an FPS game.
		/// </summary>
		/// <param name="target">The target Camera.</param>
		private void handleFpsCameraRotation(Camera target)
		{
			// if mouselook is disabled, we don't want to handle rotation this way
			if (!GameSettings.CanMouseLook) return;

			// rotate the camera around the Y axis based on mouse horizontal movement
			transform.Rotate(0,
				ControlSchemeManager.Current.Actions.LookX * ControlSchemeManager.Current.MouseSensitivity *
				Time.smoothDeltaTime * MouseLookRotationMultiplier,
				0, Space.Self);

			// rotate the camera around the X axis based on mouse vertical movement
			Transform temp = target.transform;
			_rotationX += -ControlSchemeManager.Current.Actions.LookY *
			              ControlSchemeManager.Current.MouseSensitivity * Time.smoothDeltaTime *
			              MouseLookRotationMultiplier;
			
			// make sure this angle is clamped between the min and max Y values
			_rotationX = ClampAngle(_rotationX, MinY, MaxY);
			
			// update the transform's rotation with the newly applied rotation
			var targetTransform = target.transform;
			temp.localEulerAngles = new Vector3(_rotationX, targetTransform.localEulerAngles.y, 0);
			
			// interpolate between the old rotation and the new rotation
			Quaternion.Slerp(targetTransform.rotation, temp.rotation, Time.deltaTime);
		}

		/// <summary>
		/// Handle camera rotation as in the original version of the game.
		/// </summary>
		/// <param name="target">The target Camera.</param>
		private void handleTankCameraRotation(Camera target)
		{
			Quaternion transformRotation = target.transform.rotation;
			
			// derive max positive and negative rotations for this movement
			Quaternion maxNegative = Quaternion.Euler(MaxNegativeRotation, transformRotation.eulerAngles.y,
				transformRotation.eulerAngles.z);
			Quaternion maxPositive = Quaternion.Euler(MaxPositiveRotation, transformRotation.eulerAngles.y,
				transformRotation.eulerAngles.z);
			
			// store the original orientation so we can rotate back towards it when we're not looking up or down
			Quaternion originalOrientation = Quaternion.Euler(0, transformRotation.eulerAngles.y,
				transformRotation.eulerAngles.z);

			if (ControlSchemeManager.Current.Actions.LookUp.IsPressed)
			{
				// rotate towards the max negative rotation if we're looking up
				target.transform.rotation = Quaternion.RotateTowards(transformRotation, maxNegative,
					RotationSpeed * Time.deltaTime);
			}
			
			if (ControlSchemeManager.Current.Actions.LookDown.IsPressed)
			{
				// rotate towards the max positive rotation if we're looking down
				target.transform.rotation = Quaternion.RotateTowards(transformRotation, maxPositive,
					RotationSpeed * Time.deltaTime);
			}
			
			if (!ControlSchemeManager.Current.Actions.LookUp.IsPressed &&
			    !ControlSchemeManager.Current.Actions.LookDown.IsPressed)
			{
				// rotate towards the neutral rotation orientation if we neither looking up nor down
				target.transform.rotation = Quaternion.RotateTowards(transformRotation, originalOrientation,
					RotationSpeed * Time.deltaTime);
			}
		}

		/// <summary>
		/// Clamp an angle between a min and max value.
		/// </summary>
		/// <param name="angle">The current angle.</param>
		/// <param name="min">The min angle.</param>
		/// <param name="max">The max angle.</param>
		/// <returns>The clamped angle.</returns>
		private float ClampAngle(float angle, float min, float max)
		{
			if (angle < -360F)
			{
				angle += 360F;
			}
			if (angle > 360F)
			{
				angle -= 360F;
			}
			return Mathf.Clamp(angle, min, max);
		}
	}
}