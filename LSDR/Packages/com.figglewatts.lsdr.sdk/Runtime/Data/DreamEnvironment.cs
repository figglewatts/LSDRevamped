using UnityEngine;

namespace LSDR.SDK.Data
{
    [CreateAssetMenu(menuName = "LSDR SDK/Dream environment")]
    public class DreamEnvironment : ScriptableObject
    {
        [Tooltip("The color of the fog in this environment.")]
        public Color FogColor = Color.white;

        [Tooltip("Whether to render light or dark fog. Subtractive is dark.")]
        public bool SubtractiveFog = false;

        [Tooltip("The distance at which fog starts being applied. Fog here will be at its weakest.")]
        public float FogStartDistance = 10;

        [Tooltip("The distance at which fog finishes being applied. Fog here will be at its strongest.")]
        public float FogEndDistance = 30;

        [Tooltip("The color of the sky in this environment.")]
        public Color SkyColor = Color.cyan;

        [Tooltip("Whether the sun is enabled.")]
        public bool Sun;

        [Tooltip("What color the sun/moon should be.")]
        public Color SunColor = Color.red;

        [Tooltip("The chance the sun has to be enabled. 1 is always, 0 is never.")] [Range(0, 1)]
        public float SunChance = 1;

        [Tooltip("What color the 2nd sun/moon should be.")]
        public Color SecondSunColor = Color.blue;

        [Tooltip("The chance the 2nd sun has to be enabled. 1 is always, 0 is never.")] [Range(0, 1)]
        public float SecondSunChance = 0.5f;

        [Tooltip("Whether there is a sunburst.")]
        public bool SunBurst;

        [Tooltip("The color of the sunburst.")]
        public Color SunBurstColor = Color.red;

        [Tooltip("The chance the sunburst has to be enabled. 1 is always, 0 is never.")] [Range(0, 1)]
        public float SunBurstChance = 0.5f;

        [Tooltip("Whether clouds are enabled.")]
        public bool Clouds;

        [Tooltip("The color of the clouds.")] public Color CloudsColor;

        [Tooltip("The number of clouds to spawn.")]
        public int NumberOfClouds = 6;

        [Tooltip("The chance the clouds have to be enabled. 1 is always, 0 is never.")] [Range(0, 1)]
        public float CloudsChance = 0.5f;

        [Tooltip("Whether stars are enabled.")]
        public bool Stars;

        [Tooltip("The color of the starfields.")]
        public Color StarsColor;

        [Tooltip("The number of starfields to spawn.")]
        public int NumberOfStars = 3;

        [Tooltip("The chance the stars have to be enabled. 1 is always, 0 is never.")] [Range(0, 1)]
        public float StarsChance = 0;
    }
}
