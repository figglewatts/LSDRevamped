using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Types;
using UnityEngine;
using Util;

namespace Entities.WorldObject
{
	public class ModelObject : MonoBehaviour
	{
		public string ModelSrc;
		public string LinkedLevel;
		public Color FadeColor;
		public string AudioClip;
		public float LoopDelay;

		public bool ForceFadeColor;
		public bool LinkToSpecificLevel;
		public bool DisableLinking;
		public bool IsSolid;
		public bool PlayAudio;

		public AudioSource Source;

		public static GameObject Instantiate(ENTITY e)
		{
			GameObject instantiated = new GameObject(e.Classname);
			ModelObject script = instantiated.AddComponent<ModelObject>();

			script.Source = instantiated.AddComponent<AudioSource>();

			script.ModelSrc = e.GetPropertyValue("Model src");
			script.LinkedLevel = e.GetPropertyValue("Linked level");

			script.ForceFadeColor = e.GetSpawnflagValue(0, 5);
			script.LinkToSpecificLevel = e.GetSpawnflagValue(1, 5);
			script.DisableLinking = e.GetSpawnflagValue(2, 5);
			script.IsSolid = e.GetSpawnflagValue(3, 5);
			script.PlayAudio = e.GetSpawnflagValue(4, 5);

			GameObject meshObject = IOUtil.LoadObject(script.ModelSrc, script.IsSolid);
			meshObject.transform.SetParent(instantiated.transform);

			if (script.ForceFadeColor) script.FadeColor = EntityUtil.TryParseColor("Fade color", e);
			if (script.PlayAudio)
			{
				script.StartCoroutine(IOUtil.LoadOGGIntoSource(script.AudioClip, script.Source));
				script.LoopDelay = EntityUtil.TryParseFloat("Loop delay", e);
			}

			EntityUtil.SetInstantiatedObjectTransform(e, ref instantiated);

			return instantiated;
		}
	}
}
