using UnityEngine;
using System.Collections;
using Types;
using Util;

namespace Entities.Action
{
	public class SoundAction : BaseAction
	{
		public string PathToAudioClip;
		public AudioSource Source;

		public void Start() { StartCoroutine(IOUtil.LoadOGGIntoSource(PathToAudioClip, Source)); }

		public static GameObject Instantiate(ENTITY e)
		{
			GameObject instantiated = new GameObject(e.Classname);
			SoundAction actionScript = instantiated.AddComponent<SoundAction>();
			actionScript.Name = e.GetPropertyValue("Sequence name");
			actionScript.SequencePosition = EntityUtil.TryParseInt("Sequence position", e);

			actionScript.PathToAudioClip = e.GetPropertyValue("Audio clip");

			actionScript.Source = instantiated.AddComponent<AudioSource>();

			EntityUtil.SetInstantiatedObjectTransform(e, ref instantiated);

			return instantiated;
		}
	}
}