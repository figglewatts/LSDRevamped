using LSDR.SDK.Data;
using LSDR.SDK.Editor.Util;
using UnityEditor;
using UnityEngine;

namespace LSDR.SDK
{
    public class GraphSpawnMapJournalDreamsEditorWindow : EditorWindow
    {
        public GraphSpawnMap GraphSpawnMap;

        protected DreamJournal _journal;

        public static void Show(GraphSpawnMap map)
        {
            GraphSpawnMapJournalDreamsEditorWindow window =
                GetWindow<GraphSpawnMapJournalDreamsEditorWindow>();
            window.GraphSpawnMap = map;
            window.titleContent = new GUIContent("Add from journal");
            window.CenterOnMainWindow();
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            {
                _journal = (DreamJournal)EditorGUILayout.ObjectField(GUIContent.none, _journal, typeof(DreamJournal),
                    false);

                GUILayout.FlexibleSpace();

                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Cancel"))
                    {
                        Close();
                    }

                    EditorGUI.BeginDisabledGroup(_journal == null);
                    if (GUILayout.Button("Use dreams"))
                    {
                        GraphSpawnMap.AddDreamsFromJournal(_journal);
                        Close();
                    }

                    EditorGUI.EndDisabledGroup();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }
    }
}
