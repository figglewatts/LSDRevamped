using UnityEngine;
using Game;
using InputManagement;

namespace Entities.Player
{
	/// <summary>
	/// Controls player camera motion.
	/// Rotates the camera up and down on inputs, and handles mouse look when FPS movement mode is enabled.
	/// </summary>
	public class PlayerCameraRotation : MonoBehaviour
	{
		public float RotationSpeed = 0.7F;
		public float MaxPositiveRotation = 40;
		public float MaxNegativeRotation = 320;
		public bool CanRotate = true;
		public Camera TargetCamera;
		public float MouseLookRotationMultiplier = 50; // used because deltaTime makes things real slow
		public float MaxY = 70F;
		public float MinY = -70F;

		private Quaternion _maxPos;
		private Quaternion _maxNeg;
		private Quaternion _originalRotation;

		private Transform _temp;
		private float _rotationX;

		void Start()
		{
			GameSettings.SetCursorViewState(true);

			_originalRotation = Quaternion.Euler(0, TargetCamera.transform.rotation.eulerAngles.y, TargetCamera.transform.rotation.eulerAngles.z);

			_maxNeg = Quaternion.Euler(320, TargetCamera.transform.rotation.eulerAngles.y, TargetCamera.transform.rotation.eulerAngles.z);
			_maxPos = Quaternion.Euler(40, TargetCamera.transform.rotation.eulerAngles.y, TargetCamera.transform.rotation.eulerAngles.z);
		}

		// Update is called once per frame
		void Update()
		{
			if (GameSettings.FPSMovementEnabled)
			{
				transform.Rotate(0, Input.GetAxis("Mouse X") * GameSettings.MouseSensitivityX * Time.smoothDeltaTime * MouseLookRotationMultiplier,
					0, Space.Self);

				_temp = Camera.main.transform;
				_rotationX += -Input.GetAxis("Mouse Y") * GameSettings.MouseSensitivityY * Time.smoothDeltaTime * MouseLookRotationMultiplier;
				_rotationX = ClampAngle(_rotationX, MinY, MaxY);
				_temp.transform.localEulerAngles = new Vector3(_rotationX, TargetCamera.transform.localEulerAngles.y, 0);
				Quaternion.Slerp(TargetCamera.transform.rotation, _temp.transform.rotation, Time.deltaTime);
			}
			else
			{
				_maxNeg = Quaternion.Euler(MaxNegativeRotation, TargetCamera.transform.rotation.eulerAngles.y,
					TargetCamera.transform.rotation.eulerAngles.z);
				_maxPos = Quaternion.Euler(MaxPositiveRotation, TargetCamera.transform.rotation.eulerAngles.y,
					TargetCamera.transform.rotation.eulerAngles.z);
				_originalRotation = Quaternion.Euler(0, TargetCamera.transform.rotation.eulerAngles.y,
					TargetCamera.transform.rotation.eulerAngles.z);
				if (CanRotate)
				{
					if (InputHandler.CheckButtonState("LookUp", ButtonState.HELD))
					{
						TargetCamera.transform.rotation = Quaternion.RotateTowards(TargetCamera.transform.rotation, _maxNeg,
							RotationSpeed*Time.deltaTime);
					}
					if (InputHandler.CheckButtonState("LookDown", ButtonState.HELD))
					{
						TargetCamera.transform.rotation = Quaternion.RotateTowards(TargetCamera.transform.rotation, _maxPos,
							RotationSpeed*Time.deltaTime);
					}
					if (!InputHandler.CheckButtonState("LookUp", ButtonState.HELD) &&
					    !InputHandler.CheckButtonState("LookDown", ButtonState.HELD))
					{
						TargetCamera.transform.rotation = Quaternion.RotateTowards(TargetCamera.transform.rotation, _originalRotation,
							RotationSpeed*Time.deltaTime);
					}
				}
			}
		}

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