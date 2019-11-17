using System;
using UnityEditor;
using LSDR.Dream;
using Torii.Serialization;
using Torii.Util;
using UnityEngine;

namespace LSDR.SDK
{
    public class DreamEditor : EditorWindow
    {
        private Dream.Dream _dream;
        private Vector2 _scrollPos;
        private readonly ToriiSerializer _serializer = new ToriiSerializer();
        
        [MenuItem("LSDR/Create dream")]
        public static void Init()
        {
            DreamEditor editor = (DreamEditor)EditorWindow.GetWindow(typeof(DreamEditor));
            editor.titleContent = new GUIContent("Dream");
            editor.CenterOnMainWindow();
            editor._dream = new Dream.Dream {Type = DreamType.Revamped};
            editor.Show();
        }

        public void OnGUI()
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            EditorGUILayout.LabelField("Metadata", EditorStyles.boldLabel);
            _dream.Name = EditorGUILayout.TextField(new GUIContent("Name", "The name of this dream"), 
                _dream.Name);
            _dream.Author = EditorGUILayout.TextField(new GUIContent("Author", "The author of this dream"),
                _dream.Author);
            _dream.Type = (DreamType)EditorGUILayout.EnumPopup(
                new GUIContent("Type", "The type of this dream, used to determine how to load the dream"),
                _dream.Type);
            if (_dream.Type == DreamType.Legacy)
            {
                _dream.LegacyTileMode = (LegacyTileMode)EditorGUILayout.EnumPopup(
                    new GUIContent("Tile mode", "The tiling mode of this dream"), _dream.LegacyTileMode);
            }
            if (_dream.LegacyTileMode == LegacyTileMode.Horizontal)
            {
                _dream.TileWidth = EditorGUILayout.IntField(new GUIContent("Tile width", "The width of the tile map"),
                    _dream.TileWidth);
            }
            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Graph", EditorStyles.boldLabel);
            _dream.Upperness = EditorGUILayout.IntSlider(
                new GUIContent("Upperness",
                    "What effect this dream has on the 'upper' axis of the graph, when visited"), _dream.Upperness, -9,
                9);

            _dream.Dynamicness = EditorGUILayout.IntSlider(
                new GUIContent("Dynamicness",
                    "What effect this dream has on the 'dynamic' axis of the graph, when visited"), _dream.Dynamicness,
                -9,
                9);
            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Data", EditorStyles.boldLabel);
            _dream.Level = EditorGUILayout.TextField(
                new GUIContent("Level",
                    "The path to the raw level file within the game's data to load for this dream. " +
                    "Can be an LBD directory or a TMAP."),
                _dream.Level);
            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Gameplay", EditorStyles.boldLabel);
            _dream.GreyMan =
                EditorGUILayout.Toggle(new GUIContent("Grey man", "Whether or not this dream can spawn the grey man"),
                    _dream.GreyMan);
            EditorGUILayout.Separator();
            
            EditorGUILayout.LabelField("Environment", EditorStyles.boldLabel);
            
            EditorGUILayout.Separator();
            EditorGUILayout.EndScrollView();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Export"))
            {
                var path = EditorUtility.SaveFilePanel("Export dream", "", _dream.Name + ".json", "json");
                _serializer.Serialize(_dream, path);
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
