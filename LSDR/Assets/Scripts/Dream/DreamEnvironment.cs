using Newtonsoft.Json;
using UnityEngine;

namespace LSDR.Dream
{
    [JsonObject]
    public class DreamEnvironment
    {
        /// <summary>
        /// The color of the fog in this environment.
        /// </summary>
        public Color FogColor { get; set; } = Color.gray;

        /// <summary>
        /// The color of the sky in this environment.
        /// </summary>
        public Color SkyColor { get; set; } = Color.cyan;

        /// <summary>
        /// Is the sun/moon enabled in this environment?
        /// </summary>
        public bool Sun { get; set; } = true;

        /// <summary>
        /// The color of the sun/moon in this environment.
        /// </summary>
        public Color SunColor { get; set; } = Color.red;

        /// <summary>
        /// Is the sunburst effect enabled in this environment?
        /// </summary>
        public bool SunBurst { get; set; } = false;

        /// <summary>
        /// The color of the sunburst effect in this environment.
        /// </summary>
        public Color SunBurstColor { get; set; } = Color.red;

        /// <summary>
        /// Are there clouds in this environment?
        /// </summary>
        public bool Clouds { get; set; } = false;

        /// <summary>
        /// The color of the clouds in this environment.
        /// </summary>
        public Color CloudColor { get; set; } = Color.white;

        /// <summary>
        /// The number of clouds to spawn in this environment.
        /// </summary>
        public int NumberOfClouds { get; set; } = 6;

        /// <summary>
        /// Do the clouds in this environment move around?
        /// </summary>
        public bool CloudMotion { get; set; } = false;

        /// <summary>
        /// Is the fog in this environment additive or subtractive?
        /// </summary>
        public bool SubtractiveFog { get; set; } = false;
    }
}
