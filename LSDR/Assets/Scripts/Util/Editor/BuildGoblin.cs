using System.Linq;

namespace LSDR.Util
{
    using UnityEditor;
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
                buildProject();
            }
        }

        protected void buildProject()
        {
            // Get scenes from build settings
            string[] scenes = EditorBuildSettings.scenes
                                                 .Where(scene => scene.enabled)
                                                 .Select(scene => scene.path)
                                                 .ToArray();

            var buildOpts = BuildOptions.CompressWithLz4HC | BuildOptions.StrictMode;
            if (_debug)
            {
                buildOpts |= BuildOptions.Development;
            }

            PlayerSettings.bundleVersion = _version;

            // Build for Windows
            BuildPipeline.BuildPlayer(scenes, $"../Builds/{_version}/Windows/{Application.productName}.exe",
                BuildTarget.StandaloneWindows64, buildOpts);

            // Build for Linux
            BuildPipeline.BuildPlayer(scenes, $"../Builds/{_version}/Linux/{Application.productName}.x86_64",
                BuildTarget.StandaloneLinux64, buildOpts);

            // Build for Mac
            BuildPipeline.BuildPlayer(scenes, $"../Builds/{_version}/Mac/{Application.productName}.app",
                BuildTarget.StandaloneOSX, buildOpts);

            Debug.Log("Build completed!");
        }
    }
}
