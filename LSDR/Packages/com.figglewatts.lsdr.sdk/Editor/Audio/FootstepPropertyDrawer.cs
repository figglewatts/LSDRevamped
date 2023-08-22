using LSDR.SDK.Audio;
using UnityEditor;
using UnityEngine;

namespace LSDR.SDK.Editor.Audio
{
    [CustomPropertyDrawer(typeof(Footstep))]
    public class FootstepPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Adjust the height of each property field
            position.height = EditorGUIUtility.singleLineHeight;

            // Draw the Footstep label
            EditorGUI.LabelField(position, label);

            // Indent child fields for better organization
            EditorGUI.indentLevel++;

            // Adjust position for the Sound property
            position.y += EditorGUIUtility.singleLineHeight;

            // Draw the Sound field
            EditorGUI.PropertyField(position, property.FindPropertyRelative("Sound"), new GUIContent("Sound"));

            // Adjust position for the Pitch property
            position.y += EditorGUIUtility.singleLineHeight;

            // Draw the Pitch field
            EditorGUI.PropertyField(position, property.FindPropertyRelative("Pitch"), new GUIContent("Pitch"));

            EditorGUI.indentLevel--;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Height of the label + 2 properties
            return EditorGUIUtility.singleLineHeight * 3;
        }
    }
}
