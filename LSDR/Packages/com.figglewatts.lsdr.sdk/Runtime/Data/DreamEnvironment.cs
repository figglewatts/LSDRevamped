using LSDR.SDK.Util;
using LSDR.SDK.Visual;
using UnityEngine;

namespace LSDR.SDK.Data
{
    [CreateAssetMenu(menuName = "LSDR SDK/Dream environment")]
    public class DreamEnvironment : ScriptableObject
    {
        [Tooltip("The color of the fog in this environment.")]
        public Color FogColor = Color.white;

        [Tooltip("Whether to render light or dark fog. Subtractive is dark.")]
        public bool SubtractiveFog;

        [Tooltip("The distance at which fog starts being applied. Fog here will be at its weakest.")]
        public float FogStartDistance = 10;

        [Tooltip("The distance at which fog finishes being applied. Fog here will be at its strongest.")]
        public float FogEndDistance = 30;

        [Tooltip("The height of the fog in the sky.")] [Range(min: 0, max: 1)]
        public float FogHeight = 0.24f;

        [Tooltip("The strength of the fog gradient in the sky.")] [Range(min: 0, max: 1)]
        public float FogGradient = 0.24f;

        [Tooltip("The color of the sky in this environment.")]
        public Color SkyColor = Color.cyan;

        [Tooltip("The color of the fog gradient in the sky.")]
        public Color SkyFogColor = Color.white;

        [Tooltip("What color the sun/moon should be.")]
        public Color SunColor = Color.red;

        [Tooltip("The chance the sun has to be enabled. 1 is always, 0 is never.")] [Range(min: 0, max: 1)]
        public float SunChance = 1;

        [Tooltip("The height of the sun.")]
        public float SunHeight = 2f;

        [Tooltip("The size of the sun.")]
        public float SunSize = 1f;

        [Tooltip("What color the 2nd sun/moon should be.")]
        public Color SecondSunColor = Color.blue;

        [Tooltip("The size of the 2nd sun.")]
        public float SecondSunSize = 1f;

        [Tooltip("The chance the 2nd sun has to be enabled. 1 is always, 0 is never.")] [Range(min: 0, max: 1)]
        public float SecondSunChance = 0.5f;

        [Tooltip("The offset of the 2nd sun (in terms of distance away from the first).")]
        public float SecondSunOffset = -0.15f;

        [Tooltip("The height of the 2nd sun.")]
        public float SecondSunHeight = 2f;

        [Tooltip("The color of the sunburst.")]
        public Color SunBurstColor = Color.red;

        [Tooltip("The size of the sunburst effect.")]
        public float SunBurstSize = 1f;

        [Tooltip("The chance the sunburst has to be enabled. 1 is always, 0 is never.")] [Range(min: 0, max: 1)]
        public float SunBurstChance = 0.5f;

        [Tooltip("The color of the clouds.")] public Color CloudsColor;

        [Tooltip("The number of clouds to spawn.")]
        public int NumberOfClouds = 6;

        [Tooltip("The chance the clouds have to be enabled. 1 is always, 0 is never.")] [Range(min: 0, max: 1)]
        public float CloudsChance = 0.5f;

        [Tooltip("The color of the starfields.")]
        public Color StarsColor;

        [Tooltip("The number of starfields to spawn.")]
        public int NumberOfStars = 3;

        [Tooltip("The chance the stars have to be enabled. 1 is always, 0 is never.")] [Range(min: 0, max: 1)]
        public float StarsChance;

#region Applying

        protected static readonly int _subtractiveFogPropertyId = Shader.PropertyToID("_SubtractiveFog");
        protected static readonly int _fogHeightPropertyId = Shader.PropertyToID("_FogHeight");
        protected static readonly int _fogGradientPropertyId = Shader.PropertyToID("_FogGradient");
        protected static readonly int _skyColorPropertyId = Shader.PropertyToID("_SkyColor");
        protected static readonly int _fogColorPropertyId = Shader.PropertyToID("_FogColor");

        public void Apply(bool longDrawDistance, DreamEnvironmentEffects environmentEffects)
        {
            Material skyMaterial = new Material(Shader.Find("LSDR/GradientSky"));
            skyMaterial.SetFloat(_fogHeightPropertyId, FogHeight);
            skyMaterial.SetFloat(_fogGradientPropertyId, FogGradient);
            skyMaterial.SetColor(_skyColorPropertyId, SkyColor);
            skyMaterial.SetColor(_fogColorPropertyId, SkyFogColor);

            RenderSettings.skybox = skyMaterial;
            RenderSettings.fog = true;
            RenderSettings.fogColor = FogColor;
            RenderSettings.fogStartDistance = FogStartDistance;
            RenderSettings.fogEndDistance = longDrawDistance ? FogEndDistance * 3 : FogEndDistance;
            RenderSettings.fogMode = FogMode.Linear;
            Shader.SetGlobalInt(_subtractiveFogPropertyId, SubtractiveFog ? 1 : 0);

            if (environmentEffects == null || environmentEffects.Created) return;
            if (CloudsChance > 0 && RandUtil.Float() > CloudsChance)
            {
                environmentEffects.CreateClouds(NumberOfClouds, CloudsColor);
            }

            if (SunChance > 0 && RandUtil.Float() > SunChance)
            {
                if (SunBurstChance > 0 && RandUtil.Float() > SunBurstChance)
                {
                    environmentEffects.CreateSun(SunColor, SunSize, SunHeight, 0,
                        new DreamEnvironmentEffects.SunburstConfig
                        {
                            Color = SunBurstColor,
                            Scale = SunBurstSize
                        });
                }
                else
                {
                    environmentEffects.CreateSun(SunColor, SunSize, SunHeight, 0);
                }

                if (SecondSunChance > 0 && RandUtil.Float() > SecondSunChance)
                {
                    environmentEffects.CreateSun(SecondSunColor, SecondSunSize, SecondSunHeight, SecondSunOffset);
                }
            }

            if (StarsChance > 0 && RandUtil.Float() > StarsChance)
            {
                environmentEffects.CreateStars(NumberOfStars, StarsColor);
            }
        }

#endregion
    }
}
