using System;
using System.Collections.Generic;
using LSDR.Dream;
using Torii.Serialization;
using Torii.Util;
using UnityEditor;
using UnityEngine;

namespace LSDR.SDK
{
    public class JournalEditor : EditorWindow
    {
        private DreamJournal _journal;
        private Vector2 _scrollPos;
        private bool _showEntireMenu = true;
        private readonly ToriiSerializer _serializer = new ToriiSerializer();
        private readonly Stack<int> _linkableDreamsToRemove = new Stack<int>();
        private readonly Stack<int> _firstDreamsToRemove = new Stack<int>();

        [MenuItem("LSDR/Create journal")]
        public static void Init()
        {
            JournalEditor editor = (JournalEditor)EditorWindow.GetWindow(typeof(JournalEditor));
            editor.titleContent = new GUIContent("Journal");
            editor.CenterOnMainWindow();
            editor._journal = new DreamJournal();
            editor.Show();
        }
        
        public void Update()
        {
            // remove queued items, this has to be done so we don't
            // remove one whilst mid-iteration
            while (_linkableDreamsToRemove.Count > 0)
            {
                var idx = _linkableDreamsToRemove.Pop();
                _journal.LinkableDreams.RemoveAt(idx);
            }
            while (_firstDreamsToRemove.Count > 0)
            {
                var idx = _firstDreamsToRemove.Pop();
                _journal.FirstDream.RemoveAt(idx);
            }
        }

        public void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            _showEntireMenu = EditorGUILayout.Foldout(_showEntireMenu, "Create a journal",
                new GUIStyle("foldout") {fontStyle = FontStyle.Bold});
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Import", GUILayout.Width(100)))
            {
                importExistingJournal();
            }
            if (GUILayout.Button("Export", GUILayout.Width(100)))
            {
                var path = EditorUtility.SaveFilePanel("Export journal", "", _journal.Name + ".json", "json");
                if (!string.IsNullOrEmpty(path))
                {
                    _serializer.Serialize(_journal, path);
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel++;

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            if (_showEntireMenu)
            {
                EditorGUILayout.LabelField("Metadata", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                _journal.Name = EditorGUILayout.TextField(new GUIContent("Name", "The name of this dream journal"),
                    _journal.Name);
                _journal.Author = EditorGUILayout.TextField(new GUIContent("Author", "The author of this dream journal"),
                    _journal.Author);
                EditorGUILayout.Separator();
                EditorGUI.indentLevel--;
                
                EditorGUILayout.LabelField("Journal", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Linkable dreams", 
                    "The pool of dreams we can get to when randomly linking"));
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Add"))
                {
                    _journal.LinkableDreams.Add("");
                }
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel++;
                for(int i = 0; i < _journal.LinkableDreams.Count; i++)
                {
                    drawLinkableDream(i);
                }

                EditorGUI.indentLevel--;
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("First dreams", 
                        "The pool of dreams to start on on the first day of the journal." +
                        " If empty, then it'll be a random dream from the linkable dreams. "));
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Add"))
                {
                    _journal.FirstDream.Add("");
                }
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel++;
                for (int i = 0; i < _journal.FirstDream.Count; i++)
                {
                    drawFirstDream(i);
                }
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndScrollView();

            EditorGUI.indentLevel--;
        }

        private void drawLinkableDream(int i)
        {
            EditorGUILayout.BeginHorizontal();
            _journal.LinkableDreams[i] = EditorGUILayout.TextField(
                new GUIContent("", "The path to a dream that you can link to in this journal."),
                _journal.LinkableDreams[i]);
            if (GUILayout.Button("Browse", GUILayout.Width(60)))
            {
                _journal.LinkableDreams[i] = CommonGUI.BrowseForFile("Browse for dream", 
                    new[] {"Dream JSON file", "json"}, _journal.LinkableDreams[i]);
            }

            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                _linkableDreamsToRemove.Push(i);
            }

            EditorGUILayout.EndHorizontal();
        }

        private void drawFirstDream(int i)
        {
            EditorGUILayout.BeginHorizontal();
            _journal.FirstDream[i] = EditorGUILayout.TextField(
                new GUIContent("", "The path to a dream that you should start on in this journal."),
                _journal.FirstDream[i]);
            if (GUILayout.Button("Browse", GUILayout.Width(60)))
            {
                _journal.FirstDream[i] = CommonGUI.BrowseForFile("Browse for dream", new[] {"Dream JSON file", "json"}, 
                    _journal.FirstDream[i]);
            }

            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                _firstDreamsToRemove.Push(i);
            }

            EditorGUILayout.EndHorizontal();
        }

        private void importExistingJournal()
        {
            var journalPath =
                EditorUtility.OpenFilePanelWithFilters("Open journal JSON...", "", new[] {"Journal JSON file", "json"});

            if (!string.IsNullOrEmpty(journalPath))
            {
                _journal = _serializer.Deserialize<DreamJournal>(journalPath);
            }
        }
    }
}
