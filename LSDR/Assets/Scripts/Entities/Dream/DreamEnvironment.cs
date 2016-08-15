using Types;
using UnityEngine;
using Util;

namespace Entities.Dream
{
	public class DreamEnvironment : MonoBehaviour
	{
		public int Staticity;
		public int Happiness;
		public Color SunColor;
		public Color FogColor;
		public Color SkyColor;
		public bool SpawnGreyman;
		public bool ForceSkyColor;
		public bool ForceFogColor;
		public bool MusicEnabled;
		public bool UseSun;
		public bool UseClouds;

		public void Start()
		{
			DreamDirector.HappinessAccumulator += Happiness;
			DreamDirector.StaticityAccumulator += Staticity;

			// TODO: set other values with this entity
		}

		public static GameObject Instantiate(ENTITY e)
		{
			GameObject instantiated = new GameObject(e.Classname);
			DreamEnvironment dreamEnvironment = instantiated.AddComponent<DreamEnvironment>();

			dreamEnvironment.Staticity = EntityUtil.TryParseInt("Staticity", e);
			dreamEnvironment.Happiness = EntityUtil.TryParseInt("Happiness", e);

			dreamEnvironment.SpawnGreyman = e.GetSpawnflagValue(0, 6);
			dreamEnvironment.ForceSkyColor = e.GetSpawnflagValue(1, 6);
			dreamEnvironment.ForceFogColor = e.GetSpawnflagValue(2, 6);
			dreamEnvironment.MusicEnabled = e.GetSpawnflagValue(3, 6);
			dreamEnvironment.UseSun = e.GetSpawnflagValue(4, 6);
			dreamEnvironment.UseClouds = e.GetSpawnflagValue(5, 6);

			if (dreamEnvironment.ForceSkyColor) dreamEnvironment.SkyColor = EntityUtil.TryParseColor("Sky color", e);
			if (dreamEnvironment.ForceFogColor) dreamEnvironment.FogColor = EntityUtil.TryParseColor("Fog color", e);

			dreamEnvironment.SunColor = EntityUtil.TryParseColor("Sun color", e);

			EntityUtil.SetInstantiatedObjectTransform(e, ref instantiated);

			return instantiated;
		}
	}
}
