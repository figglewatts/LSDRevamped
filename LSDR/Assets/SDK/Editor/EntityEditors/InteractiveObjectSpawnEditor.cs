using LSDR.Entities.Original;
using UnityEditor;
using UnityEngine;

namespace LSDR.SDK.EntityEditors
{
    [CustomEditor(typeof(InteractiveObjectSpawn))]
    public class InteractiveObjectSpawnEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            InteractiveObjectSpawn obj = (InteractiveObjectSpawn)target;

            DrawDefaultInspector();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Show object"))
            {
                obj.CreateObject();
            }

            if (GUILayout.Button("Hide object"))
            {
                obj.RemoveObject();
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}
