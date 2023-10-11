using System;
using System.Collections;
using LSDR.SDK.Audio;
using UnityEngine;

namespace LSDR.SDK.Entities
{
    [RequireComponent(typeof(AudioSource))]
    public class DreamAudio : BaseEntity
    {
        public AudioClip Clip;
        public bool Spatial = true;
        public float FullVolumeRangeUnits = 3;
        public float RangeUnits = 10;
        public float Pitch = 1;
        [Range(0, 1)] public float Volume = 1;
        public float DelayBetweenPlaysSeconds = 1;
        public float PlayClipForSeconds = 0;
        public float DelayBeforePlayingSeconds = 0;
        public bool WaitUntilAudibleBeforePlaying = true;
        public bool Loop = false;
        public bool OneShot = false;
        public bool ControlledWithScript = false;

        protected AudioSource _audioSource;
        protected Coroutine _audioPlayerCoroutine;
        protected bool _playing = false;
        protected float _thinkInterval = 0.5f;
        protected float _t = 0;
        protected GameObject _player;

        public override void Init()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.outputAudioMixerGroup = MixerGroupProviderManager.Managed.GetMixerGroup("SFX");

            _player = EntityIndex.Instance.Get("__player");
        }

        public void OnEnable()
        {
            if (ControlledWithScript) return;
            StartPlaying();
        }

        public void OnDisable()
        {
            StopPlaying();
        }

        public void Update()
        {
            if (_player == null) return;

            if (_playing || ControlledWithScript)
                return; // no need to update if we're already playing or controlled externally

            // only update at set interval
            if (_t < _thinkInterval)
            {
                _t += Time.deltaTime;
                return;
            }
            _t = 0;

            // wait until we're within the audible range before starting to play the audio
            var distanceToPlayer = (_player.transform.position - transform.position).sqrMagnitude;
            if (distanceToPlayer < (RangeUnits * RangeUnits))
            {
                _playing = true;
            }
        }

        public override void OnValidate()
        {
            base.OnValidate();

            _audioSource = GetComponent<AudioSource>();
            _audioSource.clip = Clip;
            _audioSource.spatialBlend = Spatial ? 1 : 0;
            _audioSource.rolloffMode = AudioRolloffMode.Linear;
            _audioSource.minDistance = FullVolumeRangeUnits;
            _audioSource.maxDistance = RangeUnits;
            _audioSource.playOnAwake = false;
            _audioSource.loop = false;
            _audioSource.pitch = Pitch;
            _audioSource.volume = Volume;
            _audioSource.loop = Loop;
        }

        public void StartPlaying()
        {
            if (Loop || OneShot)
            {
                _audioSource.Play();
            }
            else
            {
                _audioPlayerCoroutine = StartCoroutine(audioPlayer());
            }
        }

        public void StopPlaying()
        {
            if (_audioPlayerCoroutine != null)
            {
                StopCoroutine(_audioPlayerCoroutine);
                _audioPlayerCoroutine = null;
            }
        }

        protected IEnumerator audioPlayer()
        {
            if (!WaitUntilAudibleBeforePlaying || ControlledWithScript) _playing = true;

            // wait until we should play
            while (!_playing) yield return null;

            while (true)
            {
                if (DelayBeforePlayingSeconds > 0) yield return new WaitForSeconds(DelayBeforePlayingSeconds);

                // play the audio
                _audioSource.Play();

                // stop the audio if that setting is enabled
                if (PlayClipForSeconds > 0)
                {
                    yield return new WaitForSeconds(PlayClipForSeconds);
                    _audioSource.Stop();
                }

                // handle waiting between audio plays
                if (DelayBetweenPlaysSeconds <= 0) yield return null; // ensure we at least yield once
                else yield return new WaitForSeconds(DelayBetweenPlaysSeconds);
            }
        }
    }
}
