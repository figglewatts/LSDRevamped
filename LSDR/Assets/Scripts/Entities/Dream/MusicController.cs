using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Types;
using UnityEngine;
using Util;

namespace Entities.Dream
{
	public class MusicController : MonoBehaviour
	{
		public string MusicDirectory;
		public bool UseSubDirectories;

		public AudioSource Source;

		public static GameObject Instantiate(ENTITY e)
		{
			GameObject instantiated = new GameObject(e.Classname);
			MusicController script = instantiated.AddComponent<MusicController>();

			script.Source = instantiated.AddComponent<AudioSource>();

			script.MusicDirectory = e.GetPropertyValue("Music directory");

			script.UseSubDirectories = e.GetSpawnflagValue(0, 1);

			// TODO: load audio from directory

			EntityUtil.SetInstantiatedObjectTransform(e, ref instantiated);

			return instantiated;
		}
	}
}
