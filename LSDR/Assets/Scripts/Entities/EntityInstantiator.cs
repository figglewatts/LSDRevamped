using System;
using Types;
using UnityEngine;
using Util;

namespace Entities
{
	public static class EntityInstantiator
	{
		public static GameObject Instantiate(ENTITY e)
		{
			// TODO: more complete implementation

			GameObject entityObject;
			switch (e.Classname)
			{
				case "!map":
				{
					entityObject = IOUtil.LoadMap(e.GetPropertyValue("Map src"), true);
					break;
				}
				case "!model":
				{
					entityObject = IOUtil.LoadObject(e.GetPropertyValue("Model src"), true);
					break;
				}
				default:
				{
					Debug.LogWarning("Could not instantiate entity with classname " + e.Classname);
					entityObject = new GameObject(e.Classname);
					break;
				}
			}

			return entityObject;
		}
	}
}
