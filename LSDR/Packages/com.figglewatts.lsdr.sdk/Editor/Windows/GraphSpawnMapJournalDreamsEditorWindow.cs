using LSDR.SDK.Data;
using LSDR.SDK.Editor.Util;
using UnityEditor;
using UnityEngine;

namespace LSDR.SDK.Editor.Windows
{
    public class GraphSpawnMapJournalDreamsEditorWindow : EditorWindow
    {
        protected GraphSpawnMap _graphSpawnMap;
        protected DreamJournal _journal;

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            {
                _journal = (DreamJournal)EditorGUILayout.ObjectField(GUIContent.none, _journal, typeof(DreamJournal),
                    allowSceneObjects: false);

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
                        _graphSpawnMap.AddDreamsFromJournal(_journal);
                        Close();
                    }

                    EditorGUI.EndDisabledGroup();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        public static void Show(GraphSpawnMap map)
        {
            GraphSpawnMapJournalDreamsEditorWindow window =
                GetWindow<GraphSpawnMapJournalDreamsEditorWindow>();
            window._graphSpawnMap = map;
            window.titleContent = new GUIContent("Add from journal");
            window.CenterOnMainWindow();
            window.Show();
        }
    }
}
