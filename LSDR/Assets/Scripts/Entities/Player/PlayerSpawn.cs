using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Types;
using UnityEngine;
using Util;

namespace Entities.Player
{
	public class PlayerSpawn : MonoBehaviour
	{
		public string Name;
		public bool ExcludeFromRandomSpawns;

		public static GameObject Instantiate(ENTITY e)
		{
			GameObject instantiated = new GameObject(e.Classname);
			PlayerSpawn script = instantiated.AddComponent<PlayerSpawn>();

			script.Name = e.GetPropertyValue("Name");

			script.ExcludeFromRandomSpawns = e.GetSpawnflagValue(0, 1);

			EntityUtil.SetInstantiatedObjectTransform(e, ref instantiated);

			return instantiated;
		}
	}
}
