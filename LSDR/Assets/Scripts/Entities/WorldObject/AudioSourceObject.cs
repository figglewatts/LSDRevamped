using System;
using System.Collections;
using LSDR.Types;
using UnityEngine;
using UnityEngine.Audio;
using LSDR.Util;

namespace LSDR.Entities.WorldObject
{
	// TODO: AudioSourceObject is obsolete
	public class AudioSourceObject : MonoBehaviour
	{
		public string AudioClip;
		public float MinDistance;
		public float LoopDelay;

		public bool LoopAudio;

		public AudioSource Source;

		private float _audioTimer;

		private static AudioMixer _masterMixer = Resources.Load<AudioMixer>("Mixers/MasterMixer");

		public void Start() { StartCoroutine(PlayAudioCoroutine()); }

		public static GameObject Instantiate(ENTITY e)
		{
			GameObject instantiated = new GameObject(e.Classname);
			AudioSourceObject script = instantiated.AddComponent<AudioSourceObject>();

			script.Source = instantiated.AddComponent<AudioSource>();

			script.AudioClip = e.GetPropertyValue("Audio clip");
			script.MinDistance = EntityUtil.TryParseFloat("Min distance", e);

			script.LoopAudio = e.GetSpawnflagValue(0, 1);

			if (script.LoopAudio) script.LoopDelay = EntityUtil.TryParseFloat("Loop delay", e);

			script.Source.loop = script.LoopAudio;
			script.Source.minDistance = script.MinDistance;
			script.Source.spatialBlend = 1; // 3D audio
			script.Source.outputAudioMixerGroup = _masterMixer.FindMatchingGroups("SFX")[0];

			EntityUtil.SetInstantiatedObjectTransform(e, ref instantiated);

			script.StartCoroutine(IOUtil.LoadOGGIntoSource(IOUtil.PathCombine("sfx", script.AudioClip), script.Source));

			return instantiated;
		}

		private IEnumerator PlayAudioCoroutine()
		{
			if (Source.clip == null) yield return null;

			while (LoopAudio)
			{
				Source.Play();
				yield return new WaitForSeconds(Source.clip.length);
				yield return new WaitForSeconds(LoopDelay);
			}
		}
	}
}
