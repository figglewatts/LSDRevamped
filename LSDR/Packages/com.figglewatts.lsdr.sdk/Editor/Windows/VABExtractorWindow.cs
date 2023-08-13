using System;
using UnityEditor;
using UnityEngine;

namespace LSDR.SDK.Editor.Windows
{
    public class VABExtractorWindow : EditorWindow
    {
        protected string _vabPath;
        protected string _outPath;

        [MenuItem("LSDR SDK/VAB Extractor")]
        public static void ShowWindow()
        {
            GetWindow<VABExtractorWindow>("VAB Extractor").Show();
        }

        public void OnGUI()
        {
            _vabPath = browseFileField(_vabPath, "VH file", "The VH file to load", "VH");
            _outPath = browseFolderField(_outPath, "Output folder", "The folder to output samples to");

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Extract", GUILayout.Width(80)))
                {
                    extractVabSamples(_vabPath, _outPath);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        protected void extractVabSamples(string vhPath, string outPath) { }

        protected string browseFolderField(string folderPath, string label, string tooltip)
        {
            string outputFolderPath;
            EditorGUILayout.BeginHorizontal();
            {
                outputFolderPath = EditorGUILayout.TextField(
                    new GUIContent(label, tooltip), folderPath);
                if (GUILayout.Button("Browse", GUILayout.Width(width: 60),
                        GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                {
                    outputFolderPath = EditorUtility.OpenFolderPanel(label, Application.dataPath, "");
                }
            }
            EditorGUILayout.EndHorizontal();
            return outputFolderPath;
        }

        protected string browseFileField(string filePath, string label, string tooltip, string extension)
        {
            string outputFilePath;
            EditorGUILayout.BeginHorizontal();
            {
                outputFilePath = EditorGUILayout.TextField(
                    new GUIContent(label, tooltip), filePath);
                if (GUILayout.Button("Browse", GUILayout.Width(width: 60),
                        GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                {
                    outputFilePath = EditorUtility.OpenFilePanel(label, Application.dataPath, extension);
                }
            }
            EditorGUILayout.EndHorizontal();
            return outputFilePath;
        }
    }
}
