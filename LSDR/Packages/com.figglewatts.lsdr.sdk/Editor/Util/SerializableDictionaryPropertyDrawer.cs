using System;
using System.Collections;
using LSDR.SDK.Util;
using UnityEditor;
using UnityEngine;

namespace LSDR.SDK.Editor.Util
{
    [CustomPropertyDrawer(typeof(SerializableDictionary<,>), true)]
    public class SerializableDictionaryPropertyDrawer : PropertyDrawer
    {
        private bool _showAddPanel = false;
        private SerializedProperty _tempKey;
        private SerializedProperty _tempValue;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            position.y += EditorGUIUtility.singleLineHeight;

            SerializedProperty keys = property.FindPropertyRelative("_keys");
            SerializedProperty values = property.FindPropertyRelative("_values");
            _tempKey = property.FindPropertyRelative("_tempKey");
            _tempValue = property.FindPropertyRelative("_tempValue");

            EditorGUI.indentLevel++;

            // Draw dictionary elements
            float elementHeights = 0;
            for (int i = 0; i < keys.arraySize; i++)
            {
                var keyProperty = keys.GetArrayElementAtIndex(i);
                var valueProperty = values.GetArrayElementAtIndex(i);

                float originalHeight = EditorGUIUtility.singleLineHeight;

                Rect foldoutRect = new Rect(position.x,
                    position.y + (i * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing)) +
                    elementHeights, position.width - 40,
                    originalHeight);
                Rect removeButtonRect = new Rect(position.x + position.width - 30,
                    position.y + (i * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing)) +
                    elementHeights, 30, originalHeight);

                keys.GetArrayElementAtIndex(i).isExpanded = EditorGUI.Foldout(foldoutRect,
                    keys.GetArrayElementAtIndex(i).isExpanded, "Element " + i);
                if (GUI.Button(removeButtonRect, "-"))
                {
                    keys.DeleteArrayElementAtIndex(i);
                    values.DeleteArrayElementAtIndex(i);
                    break;
                }

                if (keys.GetArrayElementAtIndex(i).isExpanded)
                {
                    EditorGUI.PropertyField(
                        new Rect(position.x,
                            position.y +
                            (i + 1) * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) +
                            elementHeights,
                            position.width, originalHeight),
                        keyProperty, new GUIContent("Key"));
                    EditorGUI.PropertyField(
                        new Rect(position.x,
                            position.y +
                            (i + 2) * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) +
                            elementHeights, position.width,
                            originalHeight),
                        valueProperty, new GUIContent("Value"));
                    elementHeights += EditorGUI.GetPropertyHeight(keyProperty);
                    elementHeights += EditorGUI.GetPropertyHeight(valueProperty);
                    elementHeights += EditorGUIUtility.standardVerticalSpacing;
                }
            }

            position.y +=
                keys.arraySize * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) +
                elementHeights;

            // Add element button
            if (GUI.Button(
                    new Rect(position.x,
                        position.y,
                        position.width, EditorGUIUtility.singleLineHeight), "Add Entry"))
            {
                _showAddPanel = !_showAddPanel; // Toggle panel
            }

            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            if (_showAddPanel)
            {
                float addElementHeights =
                    EditorGUI.GetPropertyHeight(_tempKey) + EditorGUI.GetPropertyHeight(_tempValue);

                // Render the separate add panel here
                Rect keyRect = new Rect(position.x,
                    position.y,
                    position.width,
                    EditorGUIUtility.singleLineHeight);
                Rect valueRect = new Rect(position.x,
                    position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing,
                    position.width,
                    EditorGUIUtility.singleLineHeight);
                position.y += addElementHeights + EditorGUIUtility.standardVerticalSpacing * 2;
                Rect addButtonRect = new Rect(position.x,
                    position.y,
                    position.width * 0.5f,
                    EditorGUIUtility.singleLineHeight);
                Rect cancelButtonRect = new Rect(position.x + position.width * 0.5f,
                    position.y,
                    position.width * 0.5f,
                    EditorGUIUtility.singleLineHeight);

                EditorGUI.PropertyField(keyRect, _tempKey);
                EditorGUI.PropertyField(valueRect, _tempValue);

                bool keyExists =
                    ((IList)keys.GetTargetObjectOfProperty()).Contains(_tempKey.GetTargetObjectOfProperty());
                EditorGUI.BeginDisabledGroup(keyExists);
                {
                    if (GUI.Button(addButtonRect, "Add"))
                    {
                        int index = keys.arraySize;
                        keys.InsertArrayElementAtIndex(index);
                        values.InsertArrayElementAtIndex(index);

                        setValue(keys.GetArrayElementAtIndex(index), _tempKey);
                        setValue(values.GetArrayElementAtIndex(index), _tempValue);

                        _showAddPanel = false;
                    }
                }
                EditorGUI.EndDisabledGroup();

                if (GUI.Button(cancelButtonRect, "Cancel"))
                {
                    _showAddPanel = false;
                }
            }

            EditorGUI.indentLevel--;

            EditorGUI.EndProperty();
        }

        protected void setValue(SerializedProperty target, SerializedProperty value)
        {
            if (target.propertyType != value.propertyType)
            {
                Debug.LogError(
                    $"unable to set value, target property type '{target.propertyType}'" +
                    $" not match value type '{value.propertyType}'");
                return;
            }

            switch (target.propertyType)
            {
                case SerializedPropertyType.Integer:
                    target.intValue = value.intValue;
                    break;
                case SerializedPropertyType.ObjectReference:
                    target.objectReferenceValue = value.objectReferenceValue;
                    break;
                case SerializedPropertyType.ManagedReference:
                    target.managedReferenceValue = value.GetTargetObjectOfProperty();
                    break;
                // case SerializedPropertyType.Boolean:
                //     break;
                // case SerializedPropertyType.Float:
                //     break;
                // case SerializedPropertyType.String:
                //     break;
                // case SerializedPropertyType.Color:
                //     break;
                // case SerializedPropertyType.LayerMask:
                //     break;
                // case SerializedPropertyType.Enum:
                //     break;
                // case SerializedPropertyType.Vector2:
                //     break;
                // case SerializedPropertyType.Vector3:
                //     break;
                // case SerializedPropertyType.Vector4:
                //     break;
                // case SerializedPropertyType.Rect:
                //     break;
                // case SerializedPropertyType.ArraySize:
                //     break;
                // case SerializedPropertyType.Character:
                //     break;
                // case SerializedPropertyType.AnimationCurve:
                //     break;
                // case SerializedPropertyType.Bounds:
                //     break;
                // case SerializedPropertyType.Gradient:
                //     break;
                // case SerializedPropertyType.Quaternion:
                //     break;
                // case SerializedPropertyType.ExposedReference:
                //     break;
                // case SerializedPropertyType.FixedBufferSize:
                //     break;
                // case SerializedPropertyType.Vector2Int:
                //     break;
                // case SerializedPropertyType.Vector3Int:
                //     break;
                // case SerializedPropertyType.RectInt:
                //     break;
                // case SerializedPropertyType.BoundsInt:
                //     break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            target.serializedObject.ApplyModifiedProperties();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float totalHeight =
                EditorGUIUtility.singleLineHeight * 2 +
                EditorGUIUtility.standardVerticalSpacing; // Initial height for the label.

            SerializedProperty keys = property.FindPropertyRelative("_keys");
            SerializedProperty values = property.FindPropertyRelative("_values");
            SerializedProperty tempKey = property.FindPropertyRelative("_tempKey");
            SerializedProperty tempValue = property.FindPropertyRelative("_tempValue");

            for (int i = 0; i < keys.arraySize; i++)
            {
                totalHeight +=
                    EditorGUIUtility.singleLineHeight +
                    EditorGUIUtility.standardVerticalSpacing; // Height for each foldout.

                var keyProperty = keys.GetArrayElementAtIndex(i);
                var valueProperty = values.GetArrayElementAtIndex(i);
                totalHeight += EditorGUI.GetPropertyHeight(keyProperty);

                if (keys.GetArrayElementAtIndex(i).isExpanded)
                {
                    totalHeight +=
                        2 * (EditorGUIUtility.singleLineHeight +
                             EditorGUIUtility.standardVerticalSpacing);
                    totalHeight += EditorGUI.GetPropertyHeight(valueProperty);
                }
            }

            totalHeight +=
                EditorGUIUtility.singleLineHeight +
                EditorGUIUtility.standardVerticalSpacing; // Height for the "Add Entry" button.

            if (_showAddPanel)
            {
                totalHeight +=
                    3 * (EditorGUIUtility.singleLineHeight +
                         EditorGUIUtility.standardVerticalSpacing)
                    + EditorGUI.GetPropertyHeight(tempKey)
                    + EditorGUI.GetPropertyHeight(
                        tempValue); // Height for the add panel.
            }

            return totalHeight;
        }
    }
}
