using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Types;
using UnityEngine;
using Util;

namespace Entities.WorldObject
{
	public class MapObject : MonoBehaviour
	{
		public string MapSrc;
		public string LinkedLevel;
		public Color FadeColor;
		public bool ForceFadeColor;
		public bool LinkToSpecificLevel;
		public bool DisableLinking;
		public bool IsSolid;

		public static GameObject Instantiate(ENTITY e)
		{
			GameObject instantiated = new GameObject(e.Classname);
			MapObject script = instantiated.AddComponent<MapObject>();

			script.MapSrc = e.GetPropertyValue("Map src");
			script.LinkedLevel = e.GetPropertyValue("Linked level");

			script.ForceFadeColor = e.GetSpawnflagValue(0, 4);
			script.LinkToSpecificLevel = e.GetSpawnflagValue(1, 4);
			script.DisableLinking = e.GetSpawnflagValue(2, 4);
			script.IsSolid = e.GetSpawnflagValue(3, 4);

			if (script.ForceFadeColor) script.FadeColor = EntityUtil.TryParseColor("Fade color", e);

			GameObject meshObject = IOUtil.LoadMap(script.MapSrc, script.IsSolid);
			meshObject.transform.SetParent(instantiated.transform);

			EntityUtil.SetInstantiatedObjectTransform(e, ref instantiated);

			return instantiated;
		}
	}
}
