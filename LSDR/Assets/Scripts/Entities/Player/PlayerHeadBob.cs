using UnityEngine;
using System.Collections;
using Game;
using InputManagement;

namespace Entities.Player
{
	public class PlayerHeadBob : MonoBehaviour
	{
		// TODO: footstep sounds
	
		public float bobbingSpeed = 0.18F;
		public float bobbingAmount = 0.2F;
		public float midpoint = 1F;
		public float stepInterval = 0.6F;
		public Camera TargetCamera;

		private float timer = 0F;

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

			if (Mathf.Abs(vertical) == 0)
			{
				timer = 0;
			}
			else
			{
				waveslice = Mathf.Sin(timer);
				timer = timer + (bobbingSpeed*Time.deltaTime);
				if (timer > Mathf.PI*2)
				{
					timer = timer - (Mathf.PI*2);
				}
			}

			if (waveslice != 0)
			{
				float translateChange = waveslice*bobbingAmount;
				float totalAxes = Mathf.Abs(vertical);
				totalAxes = Mathf.Clamp(totalAxes, 0F, 1F);
				translateChange = totalAxes*translateChange;
				Vector3 pos = TargetCamera.transform.localPosition;
				pos.y = midpoint + translateChange;
				if (GameSettings.HeadBobEnabled)
				{
					TargetCamera.transform.localPosition = pos;
				}
			}
			else
			{
				Vector3 pos = TargetCamera.transform.localPosition;
				pos.y = midpoint;
				if (GameSettings.HeadBobEnabled)
				{
					TargetCamera.transform.localPosition = pos;
				}
			}
		}
	}
}