using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Types;
using UnityEngine;
using UnityEngine.Audio;
using Util;

namespace Entities.Trigger
{
	public class TriggerSound : MonoBehaviour
	{
		public string SoundSrc;

		public bool OnceOnly;

		public AudioSource Source;

		private bool _playedYet = false;

		private static AudioMixer _masterMixer = Resources.Load<AudioMixer>("Mixers/MasterMixer");

		public static GameObject Instantiate(ENTITY e)
		{
			GameObject instantiated = new GameObject(e.Classname);
			TriggerSound script = instantiated.AddComponent<TriggerSound>();

			script.SoundSrc = e.GetPropertyValue("Sound src");

			script.OnceOnly = e.GetSpawnflagValue(0, 1);

			script.Source = instantiated.AddComponent<AudioSource>();
			script.Source.loop = false;
			script.Source.spatialBlend = 0; // 2D audio
			script.Source.playOnAwake = false;
			script.Source.outputAudioMixerGroup = _masterMixer.FindMatchingGroups("SFX")[0];

			script.StartCoroutine(IOUtil.LoadOGGIntoSource(IOUtil.PathCombine("sfx", script.SoundSrc), script.Source));

			EntityUtil.SetInstantiatedObjectTransform(e, ref instantiated);

			instantiated.AddComponent<BoxCollider>().isTrigger = true;

			return instantiated;
		}

		public void OnTriggerEnter(Collider other)
		{
			if (OnceOnly && _playedYet) return;

			if (other.CompareTag("Player"))
			{
				Source.Play();
				_playedYet = true;
			}
		}
	}
}
