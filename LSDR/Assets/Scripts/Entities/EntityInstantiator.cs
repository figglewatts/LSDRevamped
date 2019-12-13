using LSDR.Entities.Action;
using LSDR.Entities.Dream;
using LSDR.Entities.Player;
using LSDR.Entities.Trigger;
using LSDR.Entities.WorldObject;
using LSDR.Types;
using UnityEngine;

namespace LSDR.Entities
{
	// TODO: EntityInstantiator is now obsolete
	public static class EntityInstantiator
	{
		public static GameObject Instantiate(ENTITY e)
		{
			GameObject entityObject;


			return null;
		}

		public static GameObject InstantiatePrefab(string filePath, Vector3 position = default(Vector3), Quaternion rotation = default(Quaternion))
		{
			GameObject prefab = GameObject.Instantiate(Resources.Load<GameObject>(filePath));
			prefab.transform.SetParent(DreamDirector.LoadedDreamObject.transform, true);
			prefab.transform.position = position;
			prefab.transform.rotation = rotation;
			return prefab;
		}
	}
}
