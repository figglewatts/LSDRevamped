using LSDR.SDK.Util;
using UnityEditor;
using UnityEngine;

namespace LSDR.SDK.Editor.Util
{
    [CustomPropertyDrawer(typeof(SceneProperty))]
    public class ScenePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, GUIContent.none, property);

            SerializedProperty sceneAssetProperty = property.FindPropertyRelative("sceneAsset");
            SerializedProperty scenePathProperty = property.FindPropertyRelative("scenePath");

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            if (sceneAssetProperty != null)
            {
                EditorGUI.BeginChangeCheck();
                Object value = EditorGUI.ObjectField(position, sceneAssetProperty.objectReferenceValue,
                    typeof(SceneAsset),
                    allowSceneObjects: false);
                if (EditorGUI.EndChangeCheck())
                {
                    sceneAssetProperty.objectReferenceValue = value;
                    if (sceneAssetProperty.objectReferenceValue != null)
                    {
                        scenePathProperty.stringValue =
                            AssetDatabase.GetAssetPath(sceneAssetProperty.objectReferenceValue);
                    }
                }
            }

            EditorGUI.EndProperty();
        }
    }
}
