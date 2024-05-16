using LSDR.SDK.Data;
using LSDR.SDK.Editor.Data;
using LSDR.SDK.Editor.Mod;
using LSDR.SDK.Editor.Util;
using UnityEditor;
using UnityEngine;

namespace LSDR.SDK.Editor.Windows
{
    public class LSDRevampedModBuildEditorWindow : EditorWindow
    {
        protected readonly ModBuilder _modBuilder = new ModBuilder();
        protected LSDRevampedMod _mod;
        protected string _outputPath;
        protected Vector2 _scrollPos;

        protected void OnGUI()
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            EditorGUILayout.BeginVertical();
            {
                EditorGUI.BeginDisabledGroup(disabled: true);
                {
                    EditorGUILayout.ObjectField("Mod", _mod, typeof(LSDRevampedMod), allowSceneObjects: false);
                }
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Output", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();
                {
                    _outputPath = EditorGUILayout.TextField(
                        new GUIContent("Output Folder", "The folder to output the built mod into."), _outputPath);
                    if (GUILayout.Button("Browse", GUILayout.Width(width: 60),
                            GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                    {
                        var oldOutputPath = _outputPath;
                        _outputPath = EditorUtility.OpenFolderPanel("Mod output folder", "", "");
                        if (string.IsNullOrWhiteSpace(_outputPath)) _outputPath = oldOutputPath;
                        GUI.FocusControl(null);
                        SDKData.SetRecentModOutputPath(_mod, _outputPath);
                        GUIUtility.ExitGUI();
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;

                GUILayout.FlexibleSpace();

                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    EditorGUI.BeginDisabledGroup(string.IsNullOrWhiteSpace(_outputPath));
                    if (GUILayout.Button("Build", GUILayout.Width(width: 150)))
                    {
                        _modBuilder.Build(_mod, _outputPath);
                    }

                    EditorGUI.EndDisabledGroup();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        public static void Show(LSDRevampedMod mod)
        {
            LSDRevampedModBuildEditorWindow window = GetWindow<LSDRevampedModBuildEditorWindow>();
            window._mod = mod;
            window._outputPath = SDKData.GetRecentModOutputPath(mod);
            window.titleContent = new GUIContent("Build LSDR mod");
            window.minSize = new Vector2(x: 480, y: 170);
            window.maxSize = new Vector2(x: 480, y: 170);
            window.CenterOnMainWindow();
            window.Show();
        }
    }
}
