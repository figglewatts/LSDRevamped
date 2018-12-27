using UnityEngine;
using InputManagement;

namespace Entities.Player
{
	/// <summary>
	/// Handles player walk speed and headbob speed and amplitude. 
	/// This is responsible for changing speed when sprinting.
	/// </summary>
	public class PlayerSpeed : MonoBehaviour
	{
		public float DefaultMoveSpeed = 4F;
		public float SprintMoveSpeed = 6F;
		public float SprintFastMoveSpeed = 8F;

		public float DefaultBobSpeed = 20F;
		public float DefaultBobAmount = 0.06F;
		public float SprintBobSpeed = 40F;
		public float SprintBobAmount = 0.05F;
		public float SprintFastBobSpeed = 48F;
		public float SprintFastBobAmount = 0.04F;

		private float _spaceHeldTimer;

		private PlayerHeadBob _headBob;
		private PlayerMovement _playerMovement;

		private bool _isSprinting;
		private bool _isSprintingFast;

		// Use this for initialization
		void Start()
		{
			_headBob = GetComponent<PlayerHeadBob>();
			_playerMovement = GetComponent<PlayerMovement>();
		}

		// Update is called once per frame
		void Update()
		{
			if (ControlSchemeManager.Current.Actions.Run.IsPressed)
			{
				_isSprinting = true;
				_spaceHeldTimer += Time.deltaTime;
			}
			else
			{
				_spaceHeldTimer = 0F;
			}

			if (_isSprinting)
			{
				if (_spaceHeldTimer > 10)
				{
					_isSprintingFast = true;
					_playerMovement.MovementSpeed = SprintFastMoveSpeed;
					_headBob.BobbingSpeed = SprintFastBobSpeed;
					_headBob.BobbingAmount = SprintFastBobAmount;
				}
				else if (!_isSprintingFast)
				{
					_playerMovement.MovementSpeed = SprintMoveSpeed;
					_headBob.BobbingSpeed = SprintBobSpeed;
					_headBob.BobbingAmount = SprintBobAmount;
				}
			}
			if (!_isSprinting && !_isSprintingFast)
			{
				_playerMovement.MovementSpeed = DefaultMoveSpeed;
				_headBob.BobbingSpeed = DefaultBobSpeed;
				_headBob.BobbingAmount = DefaultBobAmount;
			}

			// if space is not pressed and no movement keys are pressed
			if (!ControlSchemeManager.Current.Actions.Run.IsPressed
			    && !ControlSchemeManager.Current.Actions.Move.IsPressed)
			{
				_isSprinting = false;
				_isSprintingFast = false;
			}
		}
	}
}
