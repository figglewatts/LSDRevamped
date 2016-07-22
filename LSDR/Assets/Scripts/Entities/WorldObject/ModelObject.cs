using Types;
using UnityEngine;
using Util;

namespace Entities.WorldObject
{
	public class ModelObject : LinkableObject
	{
		public string ModelSrc;
		public string AudioClip;
		public float LoopDelay;

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

			if (!script.DisableLinking)
			{
				MeshCollider[] colliders = instantiated.GetComponentsInChildren<MeshCollider>();
				foreach (MeshCollider c in colliders)
				{
					c.gameObject.tag = "Linkable";
				}
			}

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
