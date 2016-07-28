using System;
using System.Collections;
using Types;
using UnityEngine;
using Util;

namespace Entities.WorldObject
{
	// TODO: hook into sfx volume

	public class AudioSourceObject : MonoBehaviour
	{
		public string AudioClip;
		public float MinDistance;
		public float LoopDelay;

		public bool LoopAudio;

		public AudioSource Source;

		private float _audioTimer;

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
