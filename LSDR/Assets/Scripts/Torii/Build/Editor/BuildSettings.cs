using System.IO;
using UnityEditor;
using UnityEngine;

namespace Torii.Build
{
    public class BuildSettings : ScriptableObject
    {
        internal const string BuildSettingsAssetPath = "Assets/Torii/Editor/BuildSettings.asset";


        public string OutputName;
        public string OutputPath = "Build";

        public static BuildSettings GetOrCreateSettings()
        {
            BuildSettings settings = AssetDatabase.LoadAssetAtPath<BuildSettings>(BuildSettingsAssetPath);
            if (settings == null)
            {
                settings = CreateInstance<BuildSettings>();
                settings.OutputName = Application.productName;
                Directory.CreateDirectory(Path.GetDirectoryName(BuildSettingsAssetPath));
                AssetDatabase.CreateAsset(settings, BuildSettingsAssetPath);
                AssetDatabase.SaveAssets();
            }

            return settings;
        }

        public static SerializedObject GetSerializedSettings() { return new SerializedObject(GetOrCreateSettings()); }
    }

    internal static class BuildSettingsGuiRegister
    {
        [SettingsProvider]
        public static SettingsProvider CreateBuildSettingsProvider()
        {
            SettingsProvider provider = new SettingsProvider("Project/ToriiBuildSettings", SettingsScope.Project)
            {
                label = "Torii Build Settings",
                guiHandler = searchContext =>
                {
                    SerializedObject settings = BuildSettings.GetSerializedSettings();
                    EditorGUILayout.PropertyField(settings.FindProperty("OutputName"));
                    EditorGUILayout.PropertyField(settings.FindProperty("OutputPath"));
                    settings.ApplyModifiedPropertiesWithoutUndo();
                },
                keywords = new[] { "build" }
            };
            return provider;
        }
    }
}
