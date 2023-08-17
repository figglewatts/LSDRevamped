using UnityEditor;
using UnityEngine;

namespace Torii.UnityEditor
{
    [CustomPropertyDrawer(typeof(BrowseFileSystemAttribute))]
    public class BrowseFileSystemDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            BrowseFileSystemAttribute attr = attribute as BrowseFileSystemAttribute;

            EditorGUI.BeginProperty(position, label, property);

            string name = string.IsNullOrEmpty(attr.Name)
                ? attr.Type == BrowseType.Directory
                    ? "directory"
                    : "file"
                : attr.Name;

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            if (attr.Type == BrowseType.Directory)
            {
                property.stringValue =
                    CommonGUI.RawBrowseFolderField(position, property.stringValue, $"Choose {name}");
            }
            else if (attr.Type == BrowseType.File)
            {
                property.stringValue =
                    CommonGUI.RawBrowseFileField(position, property.stringValue, $"Choose {name}", attr.FileFilters);
            }

            EditorGUI.EndProperty();
        }
    }
}
