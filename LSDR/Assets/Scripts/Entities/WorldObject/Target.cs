using Types;
using UnityEngine;
using Util;

namespace Entities.WorldObject
{
	public class Target : MonoBehaviour
	{
		public string Name;

		public static GameObject Instantiate(ENTITY e)
		{
			GameObject instantiated = new GameObject(e.Classname);
			Target script = instantiated.AddComponent<Target>();

			script.Name = e.GetPropertyValue("Name");

			EntityUtil.SetInstantiatedObjectTransform(e, ref instantiated);

			return instantiated;
		}
	}
}
