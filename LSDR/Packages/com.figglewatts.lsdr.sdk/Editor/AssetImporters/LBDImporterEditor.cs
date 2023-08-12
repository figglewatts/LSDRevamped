using UnityEditor;
using UnityEditor.Experimental.AssetImporters;

namespace LSDR.SDK.Editor.AssetImporters
{
    [CustomEditor(typeof(LBDImporter))]
    [CanEditMultipleObjects]
    public class LBDImporterEditor : ScriptedImporterEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SerializedProperty opaqueMaterialProperty = serializedObject.FindProperty("OpaqueMaterial");
            SerializedProperty transparentMaterialProperty = serializedObject.FindProperty("TransparentMaterial");
            SerializedProperty collisionProperty = serializedObject.FindProperty("Collision");
            SerializedProperty overrideProperty = serializedObject.FindProperty("UVMaterialOverrides");

            EditorGUILayout.PropertyField(opaqueMaterialProperty);
            EditorGUILayout.PropertyField(transparentMaterialProperty);
            EditorGUILayout.PropertyField(collisionProperty);
            EditorGUILayout.PropertyField(overrideProperty);

            serializedObject.ApplyModifiedProperties();
            ApplyRevertGUI();
        }
    }
}
