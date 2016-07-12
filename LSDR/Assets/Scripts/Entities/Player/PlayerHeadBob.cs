using UnityEngine;
using Game;
using InputManagement;

namespace Entities.Player
{
	/// <summary>
	/// Handles player head bobbing and footstep sounds.
	/// </summary>
	public class PlayerHeadBob : MonoBehaviour
	{
		// TODO: footstep sounds
	
		public float BobbingSpeed = 0.18F;
		public float BobbingAmount = 0.2F;
		public float Midpoint = 1F;
		public Camera TargetCamera;

		private float _timer;

		void Start() {}

		// Update is called once per frame
		void Update()
		{
			float waveslice = 0F;
			float vertical = 0;
			if (InputHandler.CheckButtonState("Forward", ButtonState.HELD))
			{
				vertical = 1;
			}
			// make sure we don't headbob whilst rotating
			if ((InputHandler.CheckButtonState("Left", ButtonState.HELD) || InputHandler.CheckButtonState("Right", ButtonState.HELD)) && GameSettings.FPSMovementEnabled)
			{
				vertical = 1;
			}
			if (InputHandler.CheckButtonState("Backward", ButtonState.HELD))
			{
				vertical = -1;
			}

			if (Mathf.Abs(vertical) < float.Epsilon)
			{
				_timer = 0;
			}
			else
			{
				waveslice = Mathf.Sin(_timer);
				_timer = _timer + (BobbingSpeed*Time.deltaTime);
				if (_timer > Mathf.PI*2)
				{
					_timer = _timer - (Mathf.PI*2);
				}
			}

			if (Mathf.Abs(waveslice) > float.Epsilon)
			{
				float translateChange = waveslice*BobbingAmount;
				float totalAxes = Mathf.Abs(vertical);
				totalAxes = Mathf.Clamp(totalAxes, 0F, 1F);
				translateChange = totalAxes*translateChange;
				Vector3 pos = TargetCamera.transform.localPosition;
				pos.y = Midpoint + translateChange;
				if (GameSettings.HeadBobEnabled)
				{
					TargetCamera.transform.localPosition = pos;
				}
			}
			else
			{
				Vector3 pos = TargetCamera.transform.localPosition;
				pos.y = Midpoint;
				if (GameSettings.HeadBobEnabled)
				{
					TargetCamera.transform.localPosition = pos;
				}
			}
		}
	}
}