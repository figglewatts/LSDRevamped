using Game;
using Types;
using UnityEngine;
using Util;

namespace Entities.WorldObject
{
	public class MapObject : LinkableObject
	{
		public string MapSrc;

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

			GameObject meshObject = IOUtil.LoadMap(script.MapSrc, ResourceLifespan.LEVEL);
			meshObject.transform.SetParent(instantiated.transform);

			if (!script.DisableLinking)
			{
				MeshCollider[] colliders = instantiated.GetComponentsInChildren<MeshCollider>();
				foreach (MeshCollider c in colliders)
				{
					c.gameObject.tag = "Linkable";
				}
			}

			EntityUtil.SetInstantiatedObjectTransform(e, ref instantiated);

			return instantiated;
		}
	}
}
