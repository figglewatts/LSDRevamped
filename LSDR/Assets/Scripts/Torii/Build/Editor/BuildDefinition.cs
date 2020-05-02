using System.IO;
using System.Reflection;
using LSDR.Game;
using LSDR.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEditor;
using UnityEngine;

namespace Torii.Build
{
    /// <summary>
    /// BuildDefinition is used to define a build of the game to create.
    /// </summary>
    [JsonObject]
    public class BuildDefinition
    {
        /// <summary>
        /// What should this build target?
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public BuildTarget Target { get; set; }
        
        /// <summary>
        /// What should the executable name be?
        /// </summary>
        public string ExecutableName { get; set; }
        
        /// <summary>
        /// What folder should we build the executable in?
        /// </summary>
        public string BuildFolder { get; set; }

        /// <summary>
        /// The full path to the executable that will be built. Created from ExecutableName, Target, and BuildFolder.
        /// </summary>
        [JsonIgnore]
        public string ExecutablePath
        {
            get
            {
                // determine if the path is absolute or relative
                // if the path is relative, then make it relative to the project folder
                bool isBuildFolderRooted = Path.IsPathRooted(BuildFolder);
                return isBuildFolderRooted
                    ? IOUtil.PathCombine(BuildFolder, Target.ToString(), ExecutableName)
                    : IOUtil.PathCombine(Application.dataPath, "../", BuildFolder, Target.ToString(), ExecutableName);
            }
        }

        /// <summary>
        /// Convert this BuildDefinition into a BuildPlayerOptions that can be used to build the game.
        /// </summary>
        /// <returns>The created BuildPlayerOptions.</returns>
        public BuildPlayerOptions ToBuildPlayerOptions()
        {
            return new BuildPlayerOptions()
            {
                scenes = getScenePaths(),
                locationPathName = ExecutablePath,
                target = Target
            };
        }
        
        /// <summary>
        /// Get the paths of all the scenes in the game.
        /// </summary>
        /// <returns>The paths of all the scenes in the game.</returns>
        private static string[] getScenePaths()
        {
            string[] scenes = new string[EditorBuildSettings.scenes.Length];
            for (int i = 0; i < scenes.Length; i++)
            {
                scenes[i] = EditorBuildSettings.scenes[i].path;
            }

            return scenes;
        }
    }
}