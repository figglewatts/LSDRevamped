using System;
using LSDR.SDK.Util;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


namespace LSDR.SDK.Editor.Util
{
    [CustomPropertyDrawer(typeof(AssetNameAttribute))]
    public class AssetNameDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            AssetNameAttribute assetNameAttribute = attribute as AssetNameAttribute;
            System.Type assetType = assetNameAttribute.AssetType;
            bool useResourcesPath = assetNameAttribute.UseResourcesPath;

            // Draw label
            EditorGUI.LabelField(new Rect(position.x, position.y, position.width / 4, position.height), label);

            // Draw TextField for string path
            string assetPath =
                EditorGUI.TextField(
                    new Rect(position.x + position.width / 4, position.y, position.width / 2, position.height),
                    property.stringValue);

            // Draw "Browse" button
            if (GUI.Button(
                    new Rect(position.x + 3 * position.width / 4, position.y, position.width / 4, position.height),
                    "Browse"))
            {
                EditorGUIUtility.ShowObjectPicker<Object>(null, false, "", 1);
            }

            string commandName = Event.current.commandName;
            if (commandName == "ObjectSelectorUpdated" && EditorGUIUtility.GetObjectPickerControlID() == 1)
            {
                Object selectedAsset = EditorGUIUtility.GetObjectPickerObject();
                assetPath = AssetDatabase.GetAssetPath(selectedAsset);
                if (useResourcesPath)
                {
                    int resourcesIndex = assetPath.IndexOf("/Resources/");
                    if (resourcesIndex >= 0)
                    {
                        assetPath = assetPath.Substring(resourcesIndex + 11);
                        int extensionIndex = assetPath.LastIndexOf('.');
                        if (extensionIndex >= 0)
                        {
                            assetPath = assetPath.Substring(0, extensionIndex);
                        }
                    }
                }
                property.stringValue = assetPath;
                Event.current.Use();
            }

            if (!string.IsNullOrEmpty(assetPath))
            {
                Object asset = AssetDatabase.LoadAssetAtPath(assetPath, assetType);
                if (asset != null)
                {
                    property.stringValue = assetPath;
                }
            }
        }
    }
}
