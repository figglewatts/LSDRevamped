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
		public Color CloudColor;
		public bool SpawnGreyman;
		public bool ForceSkyColor;
		public bool ForceFogColor;
		public bool MusicEnabled;
		public bool UseSun;
		public bool UseClouds;
		public bool UseSunburst;
		public bool ForceCloudColor;
		public bool UseGradient;
		public bool ForceSunColor;

		public void Start()
		{
			DreamDirector.HappinessAccumulator += Happiness;
			DreamDirector.StaticityAccumulator += Staticity;
		}

		public static GameObject Instantiate(ENTITY e)
		{
			GameObject instantiated = new GameObject(e.Classname);
			DreamEnvironment dreamEnvironment = instantiated.AddComponent<DreamEnvironment>();

			dreamEnvironment.Staticity = EntityUtil.TryParseInt("Staticity", e);
			dreamEnvironment.Happiness = EntityUtil.TryParseInt("Happiness", e);

			dreamEnvironment.SpawnGreyman = e.GetSpawnflagValue(0, 10);
			dreamEnvironment.ForceSkyColor = e.GetSpawnflagValue(1, 10);
			dreamEnvironment.ForceFogColor = e.GetSpawnflagValue(2, 10);
			dreamEnvironment.MusicEnabled = e.GetSpawnflagValue(3, 10);
			dreamEnvironment.UseSun = e.GetSpawnflagValue(4, 10);
			dreamEnvironment.UseClouds = e.GetSpawnflagValue(5, 10);
			dreamEnvironment.UseSunburst = e.GetSpawnflagValue(6, 10);
			dreamEnvironment.ForceCloudColor = e.GetSpawnflagValue(7, 10);
			dreamEnvironment.UseGradient = e.GetSpawnflagValue(8, 10);
			dreamEnvironment.ForceSunColor = e.GetSpawnflagValue(9, 10);

			if (dreamEnvironment.ForceSkyColor) dreamEnvironment.SkyColor = EntityUtil.TryParseColor("Sky color", e);
			if (dreamEnvironment.ForceFogColor) dreamEnvironment.FogColor = EntityUtil.TryParseColor("Fog color", e);
			if (dreamEnvironment.ForceCloudColor) dreamEnvironment.CloudColor = EntityUtil.TryParseColor("Cloud color", e);
			if (dreamEnvironment.ForceSunColor) dreamEnvironment.SunColor = EntityUtil.TryParseColor("Sun color", e);

			EntityUtil.SetInstantiatedObjectTransform(e, ref instantiated);

			GameObject.FindGameObjectWithTag("EnvironmentController")
				.GetComponent<EnvironmentController>()
				.EnvironmentEntity = dreamEnvironment;

			return instantiated;
		}
	}
}
