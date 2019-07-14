using UnityEngine;
using LSDR.Util;

namespace LSDR.Entities.Dream
{
	// TODO: make EnvironmentController obsolete by new environment features and DreamDirector refactor
	public class EnvironmentController : MonoBehaviour
	{
		[HideInInspector] public DreamEnvironment EnvironmentEntity;

		public GameObject SunDomeObject;
		public GameObject SunburstEffect;
		public GameObject GradientObject;
		public GameObject CloudParticleSystem;

		private Material _skyMaterial;
		private Material _gradientMaterial;
		private Material _sunMaterial;
		private Material _sunburstMaterial;
		private Material[] _cloudMaterials;

		// Use this for initialization
		void Awake()
		{
			DreamDirector.OnLevelFinishChange += UpdateEnvironment;

			_skyMaterial = Resources.Load<Material>("Materials/Sky/SkyBackground");
			_gradientMaterial = Resources.Load<Material>("Materials/Sky/SkyGradient");
			_sunMaterial = Resources.Load<Material>("Materials/Sky/SunTexture");
			_sunburstMaterial = Resources.Load<Material>("Materials/Sky/Sunburst");
			_cloudMaterials = Resources.LoadAll<Material>("Materials/Sky/Clouds");
		}

		public void UpdateEnvironment()
		{
			if (EnvironmentEntity == null)
			{
				Debug.LogWarning("Level has no dream_environment entity! Colors will be random.");
				SetToRandom();
				return;
			}

			Color skyColor = EnvironmentEntity.ForceSkyColor ? EnvironmentEntity.SkyColor : RandUtil.RandColor();
			Color fogColor = EnvironmentEntity.ForceFogColor ? EnvironmentEntity.FogColor : RandUtil.RandColor();
			Color sunColor = EnvironmentEntity.ForceSunColor ? EnvironmentEntity.SunColor : RandUtil.RandColor();
			Color cloudColor = EnvironmentEntity.ForceCloudColor ? EnvironmentEntity.CloudColor : Color.white;

			_skyMaterial.SetColor("_Tint", skyColor);
			RenderSettings.fogColor = new Color(fogColor.r, fogColor.g, fogColor.b, RenderSettings.fogColor.a);
			_gradientMaterial.SetColor("_Tint", fogColor);
			_sunMaterial.SetColor("_Tint", sunColor);
			_sunburstMaterial.SetColor("_Tint", sunColor);
			foreach (Material m in _cloudMaterials) m.SetColor("_Tint", cloudColor);

			SunDomeObject.SetActive(EnvironmentEntity.UseSun);
			SunburstEffect.SetActive(EnvironmentEntity.UseSunburst);
			GradientObject.SetActive(EnvironmentEntity.UseGradient);
			CloudParticleSystem.SetActive(EnvironmentEntity.UseClouds);
		}

		private void SetToRandom()
		{
			SunDomeObject.SetActive(true);
			SunburstEffect.SetActive(true);
			GradientObject.SetActive(true);
			CloudParticleSystem.SetActive(true);

			_skyMaterial.SetColor("_Tint", RandUtil.RandColor());
			Color fogColor = RandUtil.RandColor();
			// alpha must be preserved for poly clipping distance setting in shaders
			RenderSettings.fogColor = new Color(fogColor.r, fogColor.g, fogColor.b, RenderSettings.fogColor.a);
			_gradientMaterial.SetColor("_Tint", fogColor);
			Color sunColor = RandUtil.RandColor();
			_sunMaterial.SetColor("_Tint", sunColor);
			_sunburstMaterial.SetColor("_Tint", sunColor);
			foreach (Material m in _cloudMaterials) m.SetColor("_Tint", Color.white);
		}
	}
}