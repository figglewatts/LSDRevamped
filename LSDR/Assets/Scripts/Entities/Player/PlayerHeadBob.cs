using UnityEngine;
using Game;
using InputManagement;
using UnityEngine.Audio;
using Util;

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

		public AudioSource FootstepAudioSource;

		private AudioMixer _masterMixer;

		private float _timer;

		private bool _canPlayFootstepSound = true;

		void Start()
		{
			_masterMixer = Resources.Load<AudioMixer>("Mixers/MasterMixer");
		
			FootstepAudioSource = gameObject.AddComponent<AudioSource>();
			FootstepAudioSource.spatialBlend = 0; // 2d audio
			FootstepAudioSource.outputAudioMixerGroup = _masterMixer.FindMatchingGroups("SFX")[0];
			StartCoroutine(IOUtil.LoadOGGIntoSource(IOUtil.PathCombine("sfx", "SE_00003.ogg"), FootstepAudioSource));
		}

		// Update is called once per frame
		void Update()
		{
			if (!GameSettings.CanControlPlayer) return;
			
			float waveslice = 0F;
			float vertical = 0;
			if (InputHandler.CheckButtonState("Forward", ButtonState.HELD))
			{
				vertical = 1;
			}
			// make sure we don't headbob whilst rotating
			if ((InputHandler.CheckButtonState("Left", ButtonState.HELD) || InputHandler.CheckButtonState("Right", ButtonState.HELD))
				&& ControlSchemeManager.CurrentScheme.FPSMovementEnabled)
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
					_canPlayFootstepSound = true;
				}

				// if the sine wave is at the lowest part (happens at 3 * Pi divided by 2, or 1.5 * Pi)
				if (GameSettings.EnableFootstepSounds && _timer > (3*Mathf.PI)/2)
				{
					if (_canPlayFootstepSound)
					{
						FootstepAudioSource.Play();
						_canPlayFootstepSound = false;
					}
				}
			}

			if (Mathf.Abs(waveslice) > float.Epsilon)
			{
				float translateChange = waveslice*BobbingAmount;
				float totalAxes = Mathf.Abs(vertical);
				totalAxes = Mathf.Clamp(totalAxes, 0F, 1F);
				translateChange = totalAxes*translateChange;
				Vector3 pos = TargetCamera.transform.localPosition;
				pos.y = Midpoint + (GameSettings.HeadBobEnabled ? translateChange : 0);
				TargetCamera.transform.localPosition = pos;
			}
			else
			{
				Vector3 pos = TargetCamera.transform.localPosition;
				pos.y = Midpoint;
				TargetCamera.transform.localPosition = pos;
			}
		}
	}
}