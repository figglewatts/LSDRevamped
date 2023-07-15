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

            var opaqueMaterialProperty = serializedObject.FindProperty("OpaqueMaterial");
            var transparentMaterialProperty = serializedObject.FindProperty("TransparentMaterial");
            var collisionProperty = serializedObject.FindProperty("Collision");
            var overrideProperty = serializedObject.FindProperty("UVMaterialOverrides");

            EditorGUILayout.PropertyField(opaqueMaterialProperty);
            EditorGUILayout.PropertyField(transparentMaterialProperty);
            EditorGUILayout.PropertyField(collisionProperty);
            EditorGUILayout.PropertyField(overrideProperty);

            serializedObject.ApplyModifiedProperties();
            ApplyRevertGUI();
        }
    }
}
