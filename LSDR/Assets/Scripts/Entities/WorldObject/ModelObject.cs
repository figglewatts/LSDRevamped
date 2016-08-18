using Types;
using UnityEngine;
using Util;
using System;
using System.Collections;
using Entities.Action;
using Entities.Dream;
using Game;

namespace Entities.WorldObject
{
	public class ModelObject : LinkableObject
	{
		public string ModelSrc;
		public string AudioClip;
		public float LoopDelay;
		public float MinDistance;
		public string SequenceName;

		public bool PlayAudio;
		public bool HasActionSequence;

		public AudioSource Source;

		public ActionSequence ReferencedSequence;

		public void Start()
		{
			if (PlayAudio) StartCoroutine(PlayAudioCoroutine());
		}

		public static GameObject Instantiate(ENTITY e)
		{
			GameObject instantiated = new GameObject(e.Classname);
			ModelObject script = instantiated.AddComponent<ModelObject>();

			script.ModelSrc = e.GetPropertyValue("Model src");
			script.LinkedLevel = e.GetPropertyValue("Linked level");
			script.AudioClip = e.GetPropertyValue("Audio clip");
			script.MinDistance = EntityUtil.TryParseFloat("Min distance", e);
			script.SequenceName = e.GetPropertyValue("Sequence name");

			script.ForceFadeColor = e.GetSpawnflagValue(0, 6);
			script.LinkToSpecificLevel = e.GetSpawnflagValue(1, 6);
			script.DisableLinking = e.GetSpawnflagValue(2, 6);
			script.IsSolid = e.GetSpawnflagValue(3, 6);
			script.PlayAudio = e.GetSpawnflagValue(4, 6);
			script.HasActionSequence = e.GetSpawnflagValue(5, 6);

			GameObject meshObject = IOUtil.LoadObject(script.ModelSrc, script.IsSolid, ResourceLifespan.LEVEL);
			meshObject.transform.SetParent(instantiated.transform);

			script.Source = instantiated.AddComponent<AudioSource>();
			script.Source.playOnAwake = false;
			script.Source.loop = false;
			script.Source.spatialBlend = 1; // 3D audio
			script.Source.minDistance = script.MinDistance;

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
				script.StartCoroutine(IOUtil.LoadOGGIntoSource(IOUtil.PathCombine("sfx", script.AudioClip), script.Source));
				script.LoopDelay = EntityUtil.TryParseFloat("Loop delay", e);
			}
			if (script.HasActionSequence) DreamDirector.PostLoadEvent += script.PostLoad;

			EntityUtil.SetInstantiatedObjectTransform(e, ref instantiated);

			return instantiated;
		}

		private void PostLoad()
		{
			// finding objects should only be done when the level is finished loading
			// otherwise some objects may not be instantiated yet
			ReferencedSequence = ActionSequence.FindSequence(SequenceName);
			ReferencedSequence.ReferencedGameObject = gameObject;
		}

		private IEnumerator PlayAudioCoroutine()
		{
			if (Source.clip == null) yield return null;
			
			while (true)
			{
				Source.Play();
				yield return new WaitForSeconds(Source.clip.length);
				yield return new WaitForSeconds(LoopDelay);
			}
		}
	}
}
