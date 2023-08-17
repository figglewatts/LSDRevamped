using System;
using UnityEditor;
using UnityEngine;

namespace Torii.UnityEditor
{
    public static class CommonGUI
    {
        public static string BrowseForFile(string dialogTitle, string[] filters, string existingValue)
        {
            GUI.FocusControl(name: null);
            string filePath = EditorUtility.OpenFilePanelWithFilters(dialogTitle, "", filters);

            if (string.IsNullOrEmpty(filePath))
            {
                return existingValue;
            }

            if (!filePath.Contains("StreamingAssets"))
            {
                EditorUtility.DisplayDialog("File error", "Your file must be in the 'StreamingAssets' directory!",
                    "Ok");
                return existingValue;
            }

            // now remove everything before StreamingAssets path
            int indexOf = filePath.IndexOf("StreamingAssets", StringComparison.Ordinal) + "StreamingAssets".Length;
            return filePath.Substring(indexOf);
        }

        public static string BrowseForFolder(string dialogTitle, string existingValue)
        {
            GUI.FocusControl(name: null);
            string folderPath = EditorUtility.OpenFolderPanel(dialogTitle, "", "");

            if (string.IsNullOrEmpty(folderPath))
            {
                return existingValue;
            }

            if (!folderPath.Contains("StreamingAssets"))
            {
                EditorUtility.DisplayDialog("Directory error",
                    "Your directory must be in the 'StreamingAssets' directory!",
                    "Ok");
                return existingValue;
            }

            // now remove everything before StreamingAssets path
            int indexOf = folderPath.IndexOf("StreamingAssets", StringComparison.Ordinal) + "StreamingAssets".Length;
            return folderPath.Substring(indexOf);
        }

        public static string BrowseFileField(GUIContent content, string existingValue, string dialogTitle,
            string[] filters = null)
        {
            EditorGUILayout.BeginHorizontal();
            string result = EditorGUILayout.TextField(
                content,
                existingValue);
            if (GUILayout.Button("Browse", GUILayout.Width(width: 60)))
            {
                result = BrowseForFile(dialogTitle, filters, existingValue);
            }
            EditorGUILayout.EndHorizontal();
            return result;
        }

        public static string BrowseFolderField(GUIContent content, string existingValue, string dialogTitle)
        {
            EditorGUILayout.BeginHorizontal();
            string result = EditorGUILayout.TextField(
                content,
                existingValue);
            if (GUILayout.Button("Browse", GUILayout.Width(width: 60)))
            {
                result = BrowseForFolder(dialogTitle, existingValue);
            }
            EditorGUILayout.EndHorizontal();
            return result;
        }

        public static string RawBrowseFileField(Rect position,
            string existingValue,
            string dialogTitle,
            string[] filters = null)
        {
            Rect textFieldPosition = new Rect(position.x, position.y, position.width - 60, position.height);
            string result = EditorGUI.TextField(textFieldPosition, existingValue);
            Rect buttonPosition = new Rect(position.x + position.width - 60, position.y, width: 60, position.height);
            if (GUI.Button(buttonPosition, "Browse"))
            {
                result = BrowseForFile(dialogTitle, filters, existingValue);
            }

            return result;
        }

        public static string RawBrowseFolderField(Rect position, string existingValue, string dialogTitle)
        {
            Rect textFieldPosition = new Rect(position.x, position.y, position.width - 60, position.height);
            string result = EditorGUI.TextField(textFieldPosition, existingValue);
            Rect buttonPosition = new Rect(position.x + position.width - 60, position.y, width: 60, position.height);
            if (GUI.Button(buttonPosition, "Browse"))
            {
                result = BrowseForFolder(dialogTitle, existingValue);
            }

            return result;
        }

        public static bool ColorButton(GUIContent content, Color col, params GUILayoutOption[] options)
        {
            Color prevColor = GUI.backgroundColor;
            GUI.backgroundColor = col;
            bool res = GUILayout.Button(content, options);
            GUI.backgroundColor = prevColor;
            return res;
        }
    }
}
