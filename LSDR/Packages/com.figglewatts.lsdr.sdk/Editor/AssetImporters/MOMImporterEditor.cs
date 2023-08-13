using UnityEditor;


namespace LSDR.SDK.Editor.AssetImporters
{
    [CustomEditor(typeof(MOMImporter))]
    public class MOMImporterEditor : UnityEditor.AssetImporters.ScriptedImporterEditor
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
