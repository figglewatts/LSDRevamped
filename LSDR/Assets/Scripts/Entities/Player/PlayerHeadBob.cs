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
		public float BobbingSpeed = 0.18F;
		public float BobbingAmount = 0.2F;
		public float Midpoint = 1F;
		public Camera TargetCamera;

		public AudioSource FootstepAudioSource;

		private AudioMixer _masterMixer;

		private float _timer;

		private bool _canPlayFootstepSound = true;

		private Transform _targetCamTransform;

		void Start()
		{
			_masterMixer = Resources.Load<AudioMixer>("Mixers/MasterMixer");
		
			FootstepAudioSource = gameObject.AddComponent<AudioSource>();
			FootstepAudioSource.spatialBlend = 0; // 2d audio
			FootstepAudioSource.outputAudioMixerGroup = _masterMixer.FindMatchingGroups("SFX")[0];
			StartCoroutine(IOUtil.LoadOGGIntoSource(IOUtil.PathCombine("sfx", "SE_00003.ogg"), FootstepAudioSource));

			_targetCamTransform = TargetCamera.transform;
		}

		// Update is called once per frame
		void Update()
		{
			if (!GameSettings.CanControlPlayer) return;

			float sineWave = 0F;
			float forwardMovement = ControlSchemeManager.Current.Actions.MoveY.Value;

			if (Mathf.Abs(forwardMovement) < float.Epsilon)
			{
				_timer = 0;
			}
			else
			{
				// advance sine wave and increment timer
				sineWave = Mathf.Sin(_timer);
				_timer = _timer + (BobbingSpeed*Time.deltaTime);
				
				// reset timer when sine wave repeats
				if (_timer > Mathf.PI*2)
				{
					_timer = 0;
					_canPlayFootstepSound = true;
				}

				// check to see if we're at the bottom of the curve (3Pi / 2)
				if (GameSettings.CurrentSettings.EnableFootstepSounds && _timer > (Mathf.PI * 3f) / 2f)
				{
					if (_canPlayFootstepSound)
					{
						FootstepAudioSource.Play();
						_canPlayFootstepSound = false;
					}
				}
			}
			
			// update 'head' position
			if (Mathf.Abs(sineWave) > float.Epsilon)
			{
				// only compute pos change if sine wave isn't near 0
				float translateChange = sineWave * BobbingAmount;
				//translateChange *= Mathf.Abs(forwardMovement); // scale by movement amount
				Vector3 pos = _targetCamTransform.localPosition;
				pos.y = Midpoint + (GameSettings.CurrentSettings.HeadBobEnabled ? translateChange : 0);
				_targetCamTransform.localPosition = pos;
			}
			else
			{
				Vector3 pos = _targetCamTransform.localPosition;
				pos.y = Midpoint;
				_targetCamTransform.localPosition = pos;
			}
		}
	}
}