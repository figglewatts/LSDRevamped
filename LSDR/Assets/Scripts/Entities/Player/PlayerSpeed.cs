using UnityEngine;
using LSDR.InputManagement;

namespace LSDR.Entities.Player
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

		public ControlSchemeLoaderSystem ControlScheme;

		// references to headbob and movement scripts
		private PlayerHeadBob _headBob;
		private PlayerMovement _playerMovement;

		// keeps track of which sprinting state we're in
		private bool _isSprinting;

		void Start()
		{
			// get references to the scripts we need
			_headBob = GetComponent<PlayerHeadBob>();
			_playerMovement = GetComponent<PlayerMovement>();
		}

		void FixedUpdate()
		{
			// if the sprint button is pressed, we're sprinting
			if (ControlScheme.Current.Actions.Run.IsPressed && canStartSprinting())
			{
				_isSprinting = true;
			}

			if (_isSprinting)
			{
				// update values to match sprinting
				_playerMovement.MovementSpeed = SprintMoveSpeed;
				_headBob.BobbingSpeed = SprintBobSpeed;
				_headBob.BobbingAmount = SprintBobAmount;
			}
			else
			{
				// update values to match walking
				_playerMovement.MovementSpeed = DefaultMoveSpeed;
				_headBob.BobbingSpeed = DefaultBobSpeed;
				_headBob.BobbingAmount = DefaultBobAmount;
			}

			// if space is not pressed and no movement keys are pressed
			if (!ControlScheme.Current.Actions.Run.IsPressed
			    && !ControlScheme.Current.Actions.MoveY.IsPressed && !ControlScheme.Current.Actions.MoveX.IsPressed)
			{
				_isSprinting = false;
			}
		}

		private bool canStartSprinting()
		{
			return ControlScheme.Current.Actions.MoveX.IsPressed || ControlScheme.Current.Actions.MoveY.Value > 0;
		}
	}
}
