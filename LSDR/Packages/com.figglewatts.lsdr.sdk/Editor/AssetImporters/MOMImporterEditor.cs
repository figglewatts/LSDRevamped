using UnityEditor;
using UnityEditor.Experimental.AssetImporters;

namespace LSDR.SDK.Editor.AssetImporters
{
    [CustomEditor(typeof(MOMImporter))]
    public class MOMImporterEditor : ScriptedImporterEditor
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
