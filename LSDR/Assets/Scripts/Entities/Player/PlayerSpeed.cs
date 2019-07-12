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
		/// <summary>
		/// The move speed when walking normally.
		/// </summary>
		public float DefaultMoveSpeed;
		
		/// <summary>
		/// The move speed when sprinting.
		/// </summary>
		public float SprintMoveSpeed;
		
		/// <summary>
		/// The move speed when sprinting for a while.
		/// </summary>
		public float SprintFastMoveSpeed;

		/// <summary>
		/// The bob speed when walking normally.
		/// </summary>
		public float DefaultBobSpeed;
		
		/// <summary>
		/// The bob amplitude when walking normally.
		/// </summary>
		public float DefaultBobAmount;
		
		/// <summary>
		/// The bob speed when sprinting.
		/// </summary>
		public float SprintBobSpeed;
		
		/// <summary>
		/// The bob amplitude when sprinting.
		/// </summary>
		public float SprintBobAmount;
		
		/// <summary>
		/// The bob speed when sprinting for a while.
		/// </summary>
		public float SprintFastBobSpeed;
		
		/// <summary>
		/// The bob amplitude when sprinting for a while.
		/// </summary>
		public float SprintFastBobAmount;

		// timer used to figure out how long we've been sprinting for
		private float _sprintingTimer;

		// references to headbob and movement scripts
		private PlayerHeadBob _headBob;
		private PlayerMovement _playerMovement;

		// keeps track of which sprinting state we're in
		private bool _isSprinting;
		private bool _isSprintingFast;

		void Start()
		{
			// get references to the scripts we need
			_headBob = GetComponent<PlayerHeadBob>();
			_playerMovement = GetComponent<PlayerMovement>();
		}

		void Update()
		{
			// if the sprint button is pressed, we're sprinting
			if (ControlSchemeManager.Current.Actions.Run.IsPressed)
			{
				_isSprinting = true;
			}

			if (_isSprinting)
			{
				_sprintingTimer += Time.deltaTime;
				if (_sprintingTimer > 10)
				{
					// if we've been sprinting for 10 seconds, we should now sprint faster
					// update values to match sprinting faster
					_isSprintingFast = true;
					_playerMovement.MovementSpeed = SprintFastMoveSpeed;
					_headBob.BobbingSpeed = SprintFastBobSpeed;
					_headBob.BobbingAmount = SprintFastBobAmount;
				}
				else if (!_isSprintingFast)
				{
					// update values to match sprinting
					_playerMovement.MovementSpeed = SprintMoveSpeed;
					_headBob.BobbingSpeed = SprintBobSpeed;
					_headBob.BobbingAmount = SprintBobAmount;
				}
			}
			if (!_isSprinting && !_isSprintingFast)
			{
				// update values to match walking
				_playerMovement.MovementSpeed = DefaultMoveSpeed;
				_headBob.BobbingSpeed = DefaultBobSpeed;
				_headBob.BobbingAmount = DefaultBobAmount;
			}

			// if space is not pressed and no movement keys are pressed
			if (!ControlSchemeManager.Current.Actions.Run.IsPressed
			    && !ControlSchemeManager.Current.Actions.MoveY.IsPressed)
			{
				_isSprinting = false;
				_isSprintingFast = false;
			}
		}
	}
}
