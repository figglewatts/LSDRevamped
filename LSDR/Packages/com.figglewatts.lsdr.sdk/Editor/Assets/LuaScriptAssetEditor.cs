using LSDR.SDK.Assets;
using UnityEngine;
using UnityEditor;

namespace LSDR.SDK.Editor.Assets
{
    [CustomEditor(typeof(LuaScriptAsset))]
    public class LuaScriptAssetEditor : UnityEditor.Editor
    {
        private SerializedProperty scriptTextProperty;

        private void OnEnable()
        {
            // Link the serialized property to the actual field in the script
            scriptTextProperty = serializedObject.FindProperty("ScriptText");
        }

        public override void OnInspectorGUI()
        {
            // This applies the changes made in the inspector to the object
            serializedObject.Update();

            // Draw a non-editable text area for the ScriptText
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextArea(scriptTextProperty.stringValue, GUILayout.ExpandHeight(true));
            EditorGUI.EndDisabledGroup();

            // Apply changes
            serializedObject.ApplyModifiedProperties();
        }
    }
}
