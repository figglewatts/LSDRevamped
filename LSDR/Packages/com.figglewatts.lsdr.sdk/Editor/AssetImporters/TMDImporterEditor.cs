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

            SerializedProperty opaqueMaterialProperty = serializedObject.FindProperty("OpaqueMaterial");
            SerializedProperty transparentMaterialProperty = serializedObject.FindProperty("TransparentMaterial");

            EditorGUILayout.PropertyField(opaqueMaterialProperty);
            EditorGUILayout.PropertyField(transparentMaterialProperty);

            serializedObject.ApplyModifiedProperties();
            ApplyRevertGUI();
        }
    }
}
