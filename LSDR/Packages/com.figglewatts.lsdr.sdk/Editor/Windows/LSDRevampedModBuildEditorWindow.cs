using LSDR.SDK.Data;
using LSDR.SDK.Editor.Mod;
using LSDR.SDK.Editor.Util;
using UnityEditor;
using UnityEngine;

namespace LSDR.SDK.Editor.Windows
{
    public class LSDRevampedModBuildEditorWindow : EditorWindow
    {
        protected LSDRevampedMod _mod;
        protected Vector2 _scrollPos;
        protected string _outputPath;
        protected ModPlatform _platform = ModPlatform.Windows;
        protected readonly ModBuilder _modBuilder = new ModBuilder();

        public static void Show(LSDRevampedMod mod)
        {
            LSDRevampedModBuildEditorWindow window = GetWindow<LSDRevampedModBuildEditorWindow>();
            window._mod = mod;
            window.titleContent = new GUIContent("Build LSDR mod");
            window.minSize = new Vector2(480, 170);
            window.maxSize = new Vector2(480, 170);
            window.CenterOnMainWindow();
            window.Show();
        }

        protected void OnGUI()
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            EditorGUILayout.BeginVertical();
            {
                EditorGUI.BeginDisabledGroup(true);
                {
                    EditorGUILayout.ObjectField("Mod", _mod, typeof(LSDRevampedMod), false);
                }
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.Space();

                _platform = (ModPlatform)EditorGUILayout.EnumPopup("platform", _platform);

                EditorGUILayout.LabelField("Output", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();
                {
                    _outputPath = EditorGUILayout.TextField(
                        new GUIContent("Output Folder", "The folder to output the built mod into."), _outputPath);
                    if (GUILayout.Button("Browse", GUILayout.Width(60),
                            GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                    {
                        _outputPath = EditorUtility.OpenFolderPanel("Mod output folder", "", "");
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;

                GUILayout.FlexibleSpace();

                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    EditorGUI.BeginDisabledGroup(string.IsNullOrWhiteSpace(_outputPath));
                    if (GUILayout.Button("Build", GUILayout.Width(150)))
                    {
                        _modBuilder.Build(_mod, _platform, _outputPath);
                    }

                    EditorGUI.EndDisabledGroup();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
    }
}
