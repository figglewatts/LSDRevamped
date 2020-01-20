using System.Collections.Generic;
using System.IO;
using System.Reflection;
using LSDR.Game;
using UnityEditor;
using UnityEngine;
using Torii.Serialization;
using LSDR.Util;

namespace LSDR.Editor.BuildSystem
{
    /// <summary>
    /// BuildScript is used to build the game when running Unity from the command line.
    /// </summary>
    public class BuildScript
    {
        private static readonly ToriiSerializer _serializer = new ToriiSerializer();

        /// <summary>
        /// The filename of the build definition file to load.
        /// </summary>
        private const string BUILD_DEFINITION_FILE = "build_defs.json";
        
        /// <summary>
        /// Used for building the game.
        /// </summary>
        public static void Build()
        {
            Debug.Log("Loading build definitions...");
            
            // the build definition file is stored in the project folder, one above the 'Assets/' folder
            string buildDefinitionPath = IOUtil.PathCombine(Application.dataPath, "../", BUILD_DEFINITION_FILE);

            // load the build definitions
            var buildDefs = _serializer.JsonDeserialize<List<BuildDefinition>>(buildDefinitionPath);

            foreach (var buildDef in buildDefs)
            {
                Debug.Log($"Building player for target '{buildDef.Target}'...");

                var result = BuildPipeline.BuildPlayer(buildDef.ToBuildPlayerOptions());
                
                writeBuildNumber(buildDef);
                
                Debug.Log(result);
            }
        }

        /// <summary>
        /// Write the current build number to a txt file with the executable.
        /// </summary>
        /// <param name="def">The build definition we're using.</param>
        private static void writeBuildNumber(BuildDefinition def)
        {
            var buildNumber = typeof(GameLoadSystem).Assembly.GetName().Version.ToString();
            var buildNumberFilePath = IOUtil.PathCombine(Path.GetDirectoryName(def.ExecutablePath), "buildnumber.txt");
            File.WriteAllText(buildNumberFilePath, buildNumber);
            
            // we also want to write the build number to the project path so the build system can pick it up
            var projectBuildNumberFilePath = IOUtil.PathCombine(Application.dataPath, "../", "lastbuildnumber.txt");
            File.WriteAllText(projectBuildNumberFilePath, buildNumber);
        }
    }
}