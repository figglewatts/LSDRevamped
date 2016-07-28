using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities.Dream;
using Types;
using UnityEngine;
using Util;

namespace Entities.Player
{
	public class PlayerSpawn : MonoBehaviour
	{
		public string Name;
		public bool ExcludeFromRandomSpawns;
		public bool ForceSpawnOnDayOne;

		public static GameObject Instantiate(ENTITY e)
		{
			GameObject instantiated = new GameObject(e.Classname);
			PlayerSpawn script = instantiated.AddComponent<PlayerSpawn>();

			script.Name = e.GetPropertyValue("Name");
			if (script.Name.Equals(string.Empty))
			{
				Debug.LogWarning("Found player_spawn without name! Please set name in Torii.");
			}

			script.ExcludeFromRandomSpawns = e.GetSpawnflagValue(0, 2);
			script.ForceSpawnOnDayOne = e.GetSpawnflagValue(1, 2);

			EntityUtil.SetInstantiatedObjectTransform(e, ref instantiated);

			DreamDirector.PlayerSpawns.Add(script);
			if (script.ForceSpawnOnDayOne) // player spawn should be forced if it's day 1
			{
				DreamDirector.PlayerSpawnForced = true;
				DreamDirector.ForcedSpawnIndex = DreamDirector.PlayerSpawns.Count - 1;
			}

			return instantiated;
		}
	}
}
