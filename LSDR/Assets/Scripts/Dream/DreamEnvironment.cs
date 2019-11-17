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
        public Color FogColor { get; set; }

        /// <summary>
        /// The color of the sky in this environment.
        /// </summary>
        public Color SkyColor { get; set; }

        /// <summary>
        /// Is the sun/moon enabled in this environment?
        /// </summary>
        public bool Sun { get; set; }

        /// <summary>
        /// The color of the sun/moon in this environment.
        /// </summary>
        public Color SunColor { get; set; }

        /// <summary>
        /// Is the sunburst effect enabled in this environment?
        /// </summary>
        public bool SunBurst { get; set; }

        /// <summary>
        /// The color of the sunburst effect in this environment.
        /// </summary>
        public Color SunBurstColor { get; set; }

        /// <summary>
        /// Are there clouds in this environment?
        /// </summary>
        public bool Clouds { get; set; }

        /// <summary>
        /// The number of clouds to spawn in this environment.
        /// </summary>
        public int NumberOfClouds { get; set; }

        /// <summary>
        /// Do the clouds in this environment move around?
        /// </summary>
        public bool CloudMotion { get; set; }
    }
}
