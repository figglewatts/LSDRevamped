using LSDR.SDK.Data;
using LSDR.SDK.Editor.Windows;
using UnityEditor;
using UnityEngine;

namespace LSDR.SDK.Editor.Data
{
    [CustomEditor(typeof(LSDRevampedMod))]
    public class LSDRevampedModEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("Name"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Author"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Journals"));

            EditorGUILayout.Space();

            if (GUILayout.Button("Build")) LSDRevampedModBuildEditorWindow.Show(target as LSDRevampedMod);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
