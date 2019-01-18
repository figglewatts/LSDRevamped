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
		public float RotationSpeed = 0.7F;
		public float MaxPositiveRotation = 40;
		public float MaxNegativeRotation = 320;
		public bool CanRotate = true;
		public List<Camera> TargetCameras;
		public float MouseLookRotationMultiplier = 10; // used because deltaTime makes things real slow
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

			foreach (Camera c in TargetCameras)
			{
				_originalRotation = Quaternion.Euler(0, c.transform.rotation.eulerAngles.y, c.transform.rotation.eulerAngles.z);

				_maxNeg = Quaternion.Euler(MaxNegativeRotation, c.transform.rotation.eulerAngles.y, c.transform.rotation.eulerAngles.z);
				_maxPos = Quaternion.Euler(MaxPositiveRotation, c.transform.rotation.eulerAngles.y, c.transform.rotation.eulerAngles.z);
			}
			
		}

		// Update is called once per frame
		void Update()
		{
			if (!GameSettings.CanControlPlayer || GameSettings.VR) return;

			foreach (Camera c in TargetCameras)
			{
				if (ControlSchemeManager.Current.FpsControls)
				{
					if (!GameSettings.CanMouseLook) return;

                    transform.Rotate(0,
                        ControlSchemeManager.Current.Actions.LookX * ControlSchemeManager.Current.MouseSensitivity *
                        Time.smoothDeltaTime * MouseLookRotationMultiplier,
                        0, Space.Self);

					_temp = c.transform;
                    _rotationX += -ControlSchemeManager.Current.Actions.LookY *
                                  ControlSchemeManager.Current.MouseSensitivity * Time.smoothDeltaTime *
                                  MouseLookRotationMultiplier;
					_rotationX = ClampAngle(_rotationX, MinY, MaxY);
					_temp.transform.localEulerAngles = new Vector3(_rotationX, c.transform.localEulerAngles.y, 0);
					Quaternion.Slerp(c.transform.rotation, _temp.transform.rotation, Time.deltaTime);
				}
				else
				{
					_maxNeg = Quaternion.Euler(MaxNegativeRotation, c.transform.rotation.eulerAngles.y,
						c.transform.rotation.eulerAngles.z);
					_maxPos = Quaternion.Euler(MaxPositiveRotation, c.transform.rotation.eulerAngles.y,
						c.transform.rotation.eulerAngles.z);
					_originalRotation = Quaternion.Euler(0, c.transform.rotation.eulerAngles.y,
						c.transform.rotation.eulerAngles.z);
					if (CanRotate)
					{
						if (ControlSchemeManager.Current.Actions.LookUp.IsPressed)
						{
							c.transform.rotation = Quaternion.RotateTowards(c.transform.rotation, _maxNeg,
								RotationSpeed * Time.deltaTime);
						}
						if (ControlSchemeManager.Current.Actions.LookDown.IsPressed)
						{
							c.transform.rotation = Quaternion.RotateTowards(c.transform.rotation, _maxPos,
								RotationSpeed * Time.deltaTime);
						}
						if (!ControlSchemeManager.Current.Actions.LookUp.IsPressed &&
							!ControlSchemeManager.Current.Actions.LookDown.IsPressed)
						{
							c.transform.rotation = Quaternion.RotateTowards(c.transform.rotation, _originalRotation,
								RotationSpeed * Time.deltaTime);
						}
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