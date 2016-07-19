using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Types;
using UnityEngine;
using Util;

namespace Entities.Trigger
{
	public class TriggerSound : MonoBehaviour
	{
		public string SoundSrc;

		public bool OnceOnly;

		public AudioSource Source;

		public static GameObject Instantiate(ENTITY e)
		{
			GameObject instantiated = new GameObject(e.Classname);
			TriggerSound script = instantiated.AddComponent<TriggerSound>();

			script.SoundSrc = e.GetPropertyValue("Sound src");

			script.OnceOnly = e.GetSpawnflagValue(0, 1);

			script.Source = instantiated.AddComponent<AudioSource>();

			script.StartCoroutine(IOUtil.LoadOGGIntoSource(script.SoundSrc, script.Source));

			EntityUtil.SetInstantiatedObjectTransform(e, ref instantiated);

			instantiated.AddComponent<BoxCollider>().isTrigger = true;

			return instantiated;
		}
	}
}
