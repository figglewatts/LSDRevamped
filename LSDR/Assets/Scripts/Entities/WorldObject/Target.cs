using System.Collections.Generic;
using LSDR.Types;
using UnityEngine;
using LSDR.Util;

namespace LSDR.Entities.WorldObject
{
	// TODO: Target is now obsolete
	public class Target : MonoBehaviour
	{
		public static List<Target> Targets = new List<Target>();
	
		public string Name;

		public static GameObject Instantiate(ENTITY e)
		{
			GameObject instantiated = new GameObject(e.Classname);
			Target script = instantiated.AddComponent<Target>();

			script.Name = e.GetPropertyValue("Name");
			if (script.Name.Equals(string.Empty))
			{
				Debug.LogWarning("Found target entity without name! Please set target name in Torii.");
			}

			EntityUtil.SetInstantiatedObjectTransform(e, ref instantiated);

			Targets.Add(script);

			return instantiated;
		}

		public static Transform GetTargetTransform(string targetName)
		{
			foreach (Target t in Targets)
			{
				if (t.Name.Equals(targetName)) return t.transform;
			}
			Debug.LogWarning("Could not find target with name " + targetName + ", please fix in Torii");
			return null;
		}
	}
}
