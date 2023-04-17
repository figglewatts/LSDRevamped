using System;
using System.Linq;
using Unity.CodeEditor;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Torii.Build
{
    /// <summary>
    ///     BuildScript is used to build the game when running Unity from the command line.
    /// </summary>
    public class BuildScript
    {
        protected const string OUTPUT_ARG = "output";
        protected const string VERSION_ARG = "version";
        protected const string FILTER_ARG = "filter";

        /// <summary>
        ///     Attempts to get the value of --argKey from CLI args.
        ///     i.e. if the args are:
        ///     --one two --three four
        ///     then:
        ///     getArgValue("one") == "two"
        ///     getArgValue("three") == "four"
        ///     etc...
        /// </summary>
        /// <param name="argKey">The key of the arg value to get.</param>
        /// <returns>The value of the arg.</returns>
        protected static string getArgValue(string argKey)
        {
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 1; i < args.Length; i++)
            {
                // if the arg doesn't look like an '--argument' or '-argument' then skip
                if (!(args[i].StartsWith("-", StringComparison.InvariantCulture) ||
                      args[i].StartsWith("--", StringComparison.InvariantCulture)))
                    continue;

                // make sure the key matches
                string key = args[i].Trim('-');
                if (!key.Equals(argKey, StringComparison.InvariantCulture)) continue;

                // return the value (the next arg) if there is one
                if (i + 1 >= args.Length) break;
                return args[i + 1];
            }

            return null;
        }

        /// <summary>
        ///     Used for generating project solution in CI/CD.
        /// </summary>
        public static void GenerateSolution()
        {
            Debug.Log("Generating solution...");
            CodeEditor.CurrentEditor.SyncAll();
        }

        /// <summary>
        ///     Used for building the game.
        /// </summary>
        public static void Build()
        {
            Debug.Log("Loading build definitions...");

            BuildSettings buildSettings = BuildSettings.GetOrCreateSettings();
            string output = getArgValue(OUTPUT_ARG);
            if (output != null) buildSettings.OutputPath = output;
            Debug.Log($"Using output path '{buildSettings.OutputPath}'");

            string version = getArgValue(VERSION_ARG);
            if (version == null)
            {
                Debug.LogError("--version argument is required");
                EditorApplication.Exit(1);
            }

            PlayerSettings.bundleVersion = version;
            Debug.Log($"Using '{version}' as version");

            string filter = getArgValue(FILTER_ARG);
            string[] filters = null;
            if (filter != null)
            {
                Debug.LogError($"Launched with config filter '{filter}'");
                filters = filter.Split(',');
            }

            Debug.Log("Loading BuildConfigurations...");
            var buildConfigs = AssetDatabase.FindAssets("t:BuildConfiguration")
                                            .Select(AssetDatabase.GUIDToAssetPath)
                                            .Select(AssetDatabase.LoadAssetAtPath<BuildConfiguration>)
                                            .Where(conf => conf.Enabled);
            foreach (BuildConfiguration buildConf in buildConfigs)
            {
                // skip if filtered out
                if (filters != null && !filters.Contains(buildConf.Name))
                    continue;

                Debug.Log($"Building for config '{buildConf.Name}'...");
                BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
                {
                    scenes = EditorBuildSettings.scenes
                                                .Where(scene => scene.enabled)
                                                .Select(scene => scene.path)
                                                .ToArray(),
                    target = buildConf.Target,
                    options = buildConf.BuildOptions,
                    locationPathName = buildConf.GetOutputPath(buildSettings)
                };

                BuildSummary summary = BuildPipeline.BuildPlayer(buildPlayerOptions).summary;
                if (summary.result == BuildResult.Succeeded)
                {
                    Debug.Log("Build success!");
                    Debug.Log($"Size: {summary.totalSize} bytes");
                    Debug.Log($"Time: {summary.totalTime}");
                }
                else
                {
                    Debug.Log($"Build did not succeed - got result '{summary.result}'");
                    EditorApplication.Exit(1);
                }
            }
        }
    }
}
