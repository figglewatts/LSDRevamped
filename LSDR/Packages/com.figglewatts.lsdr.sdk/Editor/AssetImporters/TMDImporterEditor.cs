using UnityEditor;
using UnityEditor.Experimental.AssetImporters;

namespace LSDR.SDK.Editor.AssetImporters
{
    [CustomEditor(typeof(TMDImporter))]
    [CanEditMultipleObjects]
    public class TMDImporterEditor : ScriptedImporterEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var opaqueMaterialProperty = serializedObject.FindProperty("OpaqueMaterial");
            var transparentMaterialProperty = serializedObject.FindProperty("TransparentMaterial");

            EditorGUILayout.PropertyField(opaqueMaterialProperty);
            EditorGUILayout.PropertyField(transparentMaterialProperty);

            serializedObject.ApplyModifiedProperties();
            ApplyRevertGUI();
        }
    }
}
