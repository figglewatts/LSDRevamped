using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities.Player;
using Types;
using UnityEngine;
using Util;

namespace Entities.Trigger
{
	public class TriggerLink : MonoBehaviour
	{
		public string LinkedLevel;
		public Color ForcedLinkColor;
		public string SpawnPointName;

		public bool ForceFadeColor;
		public bool LinkToSpecificLevel;
		public bool ForceSpawnPoint;
		public bool PlayLinkSound;

		public static GameObject Instantiate(ENTITY e)
		{
			GameObject instantiated = new GameObject(e.Classname);
			TriggerLink script = instantiated.AddComponent<TriggerLink>();

			script.ForceFadeColor = e.GetSpawnflagValue(0, 4);
			script.LinkToSpecificLevel = e.GetSpawnflagValue(1, 4);
			script.ForceSpawnPoint = e.GetSpawnflagValue(2, 4);
			script.PlayLinkSound = e.GetSpawnflagValue(3, 4);

			if (script.LinkToSpecificLevel) script.LinkedLevel = IOUtil.PathCombine("levels", e.GetPropertyValue("Linked level"));
			if (script.ForceFadeColor) script.ForcedLinkColor = EntityUtil.TryParseColor("Forced link color", e);
			if (script.ForceSpawnPoint) script.SpawnPointName = e.GetPropertyValue("Spawn point name");

			EntityUtil.SetInstantiatedObjectTransform(e, ref instantiated);

			instantiated.AddComponent<BoxCollider>().isTrigger = true;

			return instantiated;
		}

		public void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				PlayerLinker linker = other.GetComponent<PlayerLinker>();
				if (LinkToSpecificLevel)
				{
					linker.Link(LinkedLevel, ForceFadeColor ? ForcedLinkColor : RandUtil.RandColor(), PlayLinkSound, ForceSpawnPoint ? SpawnPointName : "");
				}
				else
				{
					linker.Link(PlayLinkSound);
				}
			}
		}
	}
}
