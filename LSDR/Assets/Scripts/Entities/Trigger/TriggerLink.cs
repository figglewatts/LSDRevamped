using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

		public static GameObject Instantiate(ENTITY e)
		{
			GameObject instantiated = new GameObject(e.Classname);
			TriggerLink script = instantiated.AddComponent<TriggerLink>();

			script.ForceFadeColor = e.GetSpawnflagValue(0, 3);
			script.LinkToSpecificLevel = e.GetSpawnflagValue(1, 3);
			script.ForceSpawnPoint = e.GetSpawnflagValue(2, 3);

			if (script.LinkToSpecificLevel) script.LinkedLevel = e.GetPropertyValue("Linked level");
			if (script.ForceFadeColor) script.ForcedLinkColor = EntityUtil.TryParseColor("Forced link color", e);
			if (script.ForceSpawnPoint) script.SpawnPointName = e.GetPropertyValue("Spawn point name");

			EntityUtil.SetInstantiatedObjectTransform(e, ref instantiated);

			instantiated.AddComponent<BoxCollider>().isTrigger = true;

			return instantiated;
		}
	}
}
