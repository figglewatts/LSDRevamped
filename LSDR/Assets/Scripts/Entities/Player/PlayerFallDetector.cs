using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using Entities.Dream;
using Game;

namespace Entities.Player
{
	/// <summary>
	/// Used to detect whether or not a player is currently falling.
	/// </summary>
	[RequireComponent(typeof(CharacterController))]
	public class PlayerFallDetector : MonoBehaviour
	{
		// TODO: refactor PlayerFallDetector to fix falling, as well as during DreamDirector refactor
		
		[SerializeField]
		private Camera _targetCamera;
		private CharacterController _playerController;
		private float _fallTimer;
		private Quaternion _lookUpRotation;
		private bool _canRotateCamera;

		/// <summary>
		/// Max amount of time player can fall for before dream ends.
		/// </summary>
		private const float MAX_FALLING_TIME = 0.7F;

		private const float ROTATION_SPEED = 40F;
		private const float MAX_X_ROTATION = 320F;

		public void Awake()
		{
			_playerController = GetComponent<CharacterController>();
		}

		public void Update()
		{
			if (_playerController.isGrounded)
			{
				_fallTimer = 0;
				return;
			}

			_fallTimer += Time.deltaTime;
			if (!_canRotateCamera && _fallTimer > MAX_FALLING_TIME)
			{
				GameSettings.CanControlPlayer = false;
				_lookUpRotation = Quaternion.Euler(MAX_X_ROTATION, _targetCamera.transform.rotation.eulerAngles.y,
					_targetCamera.transform.rotation.eulerAngles.z);
				DreamDirector.EndDreamFall();
				_canRotateCamera = true;
			}

			if (_canRotateCamera)
			{
				_targetCamera.transform.rotation = Quaternion.RotateTowards(_targetCamera.transform.rotation, _lookUpRotation,
					ROTATION_SPEED * Time.deltaTime);
			}
		}
	}
}