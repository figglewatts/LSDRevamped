using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Types;
using UnityEngine;
using Util;

namespace Entities.WorldObject
{
	public class AudioSourceObject : MonoBehaviour
	{
		public string AudioClip;
		public float MinDistance;
		public float LoopDelay;

		public bool LoopAudio;

		public AudioSource Source;

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

			EntityUtil.SetInstantiatedObjectTransform(e, ref instantiated);

			script.StartCoroutine(IOUtil.LoadOGGIntoSource(script.AudioClip, script.Source));

			return instantiated;
		}
	}
}
