using UnityEngine;
using System.Collections;
using Entities.Player;
using InputManagement;

public class PlayerSpeed : MonoBehaviour
{
	public float defaultMoveSpeed = 4F;
	public float SprintMoveSpeed = 6F;
	public float SprintFastMoveSpeed = 8F;

	public float defaultBobSpeed = 20F;
	public float defaultBobAmount = 0.06F;
	public float SprintBobSpeed = 40F;
	public float SprintBobAmount = 0.05F;
	public float SprintFastBobSpeed = 48F;
	public float SprintFastBobAmount = 0.04F;

	private float spaceHeldTimer = 0F;

	private PlayerHeadBob headBob;
	private PlayerMovement _playerMovement;

	private bool isSprinting = false;
	private bool isSprintingFast = false;

	// Use this for initialization
	void Start()
	{
		headBob = GetComponent<PlayerHeadBob>();
		_playerMovement = GetComponent<PlayerMovement>();
	}

	// Update is called once per frame
	void Update()
	{
		if (InputHandler.CheckButtonState("Sprint", ButtonState.HELD))
		{
			isSprinting = true;
			spaceHeldTimer += Time.deltaTime;
		}
		else
		{
			spaceHeldTimer = 0F;
		}

		if (isSprinting)
		{
			if (spaceHeldTimer > 10)
			{
				isSprintingFast = true;
				_playerMovement.MovementSpeed = SprintFastMoveSpeed;
				headBob.bobbingSpeed = SprintFastBobSpeed;
				headBob.bobbingAmount = SprintFastBobAmount;
			}
			else if (!isSprintingFast)
			{
				_playerMovement.MovementSpeed = SprintMoveSpeed;
				headBob.bobbingSpeed = SprintBobSpeed;
				headBob.bobbingAmount = SprintBobAmount;
			}
		}
		if (!isSprinting && !isSprintingFast)
		{
			_playerMovement.MovementSpeed = defaultMoveSpeed;
			headBob.bobbingSpeed = defaultBobSpeed;
			headBob.bobbingAmount = defaultBobAmount;
		}

		// if space is not pressed and no movement keys are pressed
		if (!InputHandler.CheckButtonState("Sprint", ButtonState.HELD)
			&& !InputHandler.CheckButtonState("Forward", ButtonState.HELD)
			&& !InputHandler.CheckButtonState("Backward", ButtonState.HELD))
		{
			isSprinting = false;
			isSprintingFast = false;
		}
	}

}
