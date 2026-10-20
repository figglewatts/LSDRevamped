using System;
using System.Linq;

namespace LSDR.Util
{
    using UnityEditor;
    using UnityEditor.Build.Reporting;
    using UnityEngine;

    public class BuildGoblin : EditorWindow
    {
        private string _version = "";
        private Texture2D _goblinTex;
        private bool _debug = false;

        [MenuItem("LSDR SDK/BUILDGOBLIN")]
        public static void ShowWindow()
        {
            GetWindow<BuildGoblin>("BUILDGOBLIN");
        }

        public void OnEnable()
        {
            _goblinTex =
                AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/UnityEditor/Resources/Textures/goblin.png");
        }

        public void OnGUI()
        {
            // Display Texture
            if (_goblinTex != null)
            {
                float viewWidth = position.width;
                GUILayout.Box("", GUILayout.Width(viewWidth), GUILayout.Height(viewWidth));
                Rect lastRect = GUILayoutUtility.GetLastRect();
                GUI.DrawTexture(lastRect, _goblinTex, ScaleMode.ScaleToFit);
            }

            _debug = EditorGUILayout.Toggle("Debug", _debug);

            // Display version text field
            _version = EditorGUILayout.TextField("Version", _version);

            // Display build button
            if (GUILayout.Button("BUILD (goblin)", GUILayout.Height(50)))
            {
                Build(_version, _debug);
            }
        }

        // for building from the command line:
        // Unity -batchmode -quit -executeMethod LSDR.Util.BuildGoblin.BuildFromCommandLine -buildVersion 1.0.0 [-development]
        public static void BuildFromCommandLine()
        {
            string[] args = Environment.GetCommandLineArgs();
            int versionArg = Array.IndexOf(args, "-buildVersion");
            if (versionArg < 0 || versionArg + 1 >= args.Length)
            {
                Debug.LogError("No version given, pass one with -buildVersion");
                EditorApplication.Exit(1);
                return;
            }

            bool succeeded = Build(args[versionArg + 1], args.Contains("-development"));
            EditorApplication.Exit(succeeded ? 0 : 1);
        }

        public static bool Build(string version, bool debug)
        {
            // Get scenes from build settings
            string[] scenes = EditorBuildSettings.scenes
                                                 .Where(scene => scene.enabled)
                                                 .Select(scene => scene.path)
                                                 .ToArray();

            var buildOpts = BuildOptions.CompressWithLz4HC | BuildOptions.StrictMode;
            if (debug)
            {
                buildOpts |= BuildOptions.Development;
            }

            PlayerSettings.bundleVersion = version;

            // build every platform even if one fails, so all the failures show up at once
            bool succeeded =
                buildPlatform(scenes, $"../Builds/{version}/Windows/{Application.productName}.exe",
                    BuildTarget.StandaloneWindows64, buildOpts) &
                buildPlatform(scenes, $"../Builds/{version}/Linux/{Application.productName}.x86_64",
                    BuildTarget.StandaloneLinux64, buildOpts) &
                buildPlatform(scenes, $"../Builds/{version}/Mac/{Application.productName}.app",
                    BuildTarget.StandaloneOSX, buildOpts);

            Debug.Log(succeeded ? "Build completed!" : "Build failed for at least one platform");
            return succeeded;
        }

        protected static bool buildPlatform(string[] scenes, string path, BuildTarget target, BuildOptions options)
        {
            BuildReport report = BuildPipeline.BuildPlayer(scenes, path, target, options);
            Debug.Log($"Building for {target}: {report.summary.result}");
            return report.summary.result == BuildResult.Succeeded;
        }
    }
}
