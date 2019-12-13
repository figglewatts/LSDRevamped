using System;
using System.Collections.Generic;
using System.Net.Configuration;
using UnityEditor;
using LSDR.Dream;
using Torii.Serialization;
using Torii.UnityEditor;
using Torii.Util;
using UnityEngine;

namespace LSDR.SDK
{
    public class DreamEditor : EditorWindow
    {
        // TODO: environment preview
        
        private Dream.Dream _dream;
        private Vector2 _scrollPos;
        private readonly ToriiSerializer _serializer = new ToriiSerializer();
        private List<bool> _dreamEnvironmentFoldoutStates = new List<bool>();
        private Stack<int> _dreamEnvironmentsToRemove = new Stack<int>();
        private bool _showEntireMenu = true;

        [MenuItem("LSDR/Create dream")]
        public static void Init()
        {
            DreamEditor editor = (DreamEditor)EditorWindow.GetWindow(typeof(DreamEditor));
            editor.titleContent = new GUIContent("Dream");
            editor.CenterOnMainWindow();
            editor._dream = new Dream.Dream();
            editor._dreamEnvironmentFoldoutStates = new List<bool>();
            editor._dreamEnvironmentsToRemove = new Stack<int>();
            editor.Show();
        }

        public void Update()
        {
            // remove queued dream environments, this has to be done so we don't
            // remove one whilst mid-iteration
            while (_dreamEnvironmentsToRemove.Count > 0)
            {
                var idx = _dreamEnvironmentsToRemove.Pop();
                _dream.Environments.RemoveAt(idx);
                _dreamEnvironmentFoldoutStates.RemoveAt(idx);
            }
        }

        public void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            _showEntireMenu = EditorGUILayout.Foldout(_showEntireMenu, "LSDR Dream Editor",
                new GUIStyle("foldout") {fontStyle = FontStyle.Bold});
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Import", GUILayout.Width(100)))
            {
                importExistingDream();
            }
            if (GUILayout.Button("Export", GUILayout.Width(100)))
            {
                var path = EditorUtility.SaveFilePanel("Export dream", "", _dream.Name + ".json", "json");

                if (!string.IsNullOrEmpty(path))
                {
                    _serializer.Serialize(_dream, path);
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel++;

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            if (_showEntireMenu)
            {
                EditorGUILayout.LabelField("Metadata", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
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
                    _dream.TileWidth = EditorGUILayout.IntField(
                        new GUIContent("Tile width", "The width of the tile map"),
                        _dream.TileWidth);
                }

                EditorGUI.indentLevel--;
                EditorGUILayout.Separator();

                EditorGUILayout.LabelField("Graph", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                _dream.Upperness = EditorGUILayout.IntSlider(
                    new GUIContent("Upperness",
                        "What effect this dream has on the 'upper' axis of the graph, when visited"), _dream.Upperness,
                    -9,
                    9);

                _dream.Dynamicness = EditorGUILayout.IntSlider(
                    new GUIContent("Dynamicness",
                        "What effect this dream has on the 'dynamic' axis of the graph, when visited"),
                    _dream.Dynamicness,
                    -9,
                    9);
                EditorGUI.indentLevel--;
                EditorGUILayout.Separator();

                EditorGUILayout.LabelField("Data", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                _dream.Level = CommonGUI.BrowseFileField(new GUIContent("Level",
                        "The path to the raw level file within the game's data to load for this dream."), _dream.Level,
                    "Choose a level file", new[] {"Torii map files", "tmap"});

                if (_dream.Type == DreamType.Legacy)
                {
                    _dream.LBDFolder = CommonGUI.BrowseFolderField(
                        new GUIContent("LBD folder", "The path to the LBD folder to load for this Dream."),
                        _dream.LBDFolder, "Choose an LBD folder.");
                }

                EditorGUI.indentLevel--;
                EditorGUILayout.Separator();

                EditorGUILayout.LabelField("Gameplay", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                _dream.GreyMan =
                    EditorGUILayout.Toggle(
                        new GUIContent("Grey man", "Whether or not this dream can spawn the grey man"),
                        _dream.GreyMan);
                EditorGUI.indentLevel--;
                EditorGUILayout.Separator();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Environment", EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Add"))
                {
                    addNewDreamEnvironment();
                }

                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel++;
                for (int i = 0; i < _dream.Environments.Count; i++)
                {
                    drawDreamEnvironmentGUI(i);
                }

                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndScrollView();

            EditorGUI.indentLevel--;
        }
        
        private void importExistingDream()
        {
            var dreamPath =
                EditorUtility.OpenFilePanelWithFilters("Open dream JSON...", "", new[] {"Dream JSON file", "json"});

            if (!string.IsNullOrEmpty(dreamPath))
            {
                _dream = _serializer.Deserialize<Dream.Dream>(dreamPath);
                _dreamEnvironmentFoldoutStates.Clear();
                foreach (var env in _dream.Environments)
                {
                    _dreamEnvironmentFoldoutStates.Add(false);
                }
            }
        }

        private void addNewDreamEnvironment()
        {
            _dream.Environments.Add(new DreamEnvironment());
            _dreamEnvironmentFoldoutStates.Add(false);
        }

        private void drawDreamEnvironmentGUI(int envIndex)
        {
            var env = _dream.Environments[envIndex];

            EditorGUILayout.BeginHorizontal();
            _dreamEnvironmentFoldoutStates[envIndex] =
                EditorGUILayout.Foldout(_dreamEnvironmentFoldoutStates[envIndex], envIndex.ToString());
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Remove"))
            {
                _dreamEnvironmentsToRemove.Push(envIndex);
            }
            EditorGUILayout.EndHorizontal();
            if (_dreamEnvironmentFoldoutStates[envIndex])
            {
                EditorGUI.indentLevel++;
                env.FogColor =
                    EditorGUILayout.ColorField(new GUIContent("Fog color", "The color of the fog in this environment"),
                        env.FogColor);
                env.SkyColor =
                    EditorGUILayout.ColorField(new GUIContent("Sky color", "The color of the sky in this environment"),
                        env.SkyColor);
                env.Sun = EditorGUILayout.Toggle(new GUIContent("Sun", "Is the sun/moon enabled in this environment?"),
                    env.Sun);
                if (env.Sun)
                {
                    env.SunColor = EditorGUILayout.ColorField(
                        new GUIContent("Sun/moon color", "The color of the sun/moon in this environment"),
                        env.SunColor);
                    env.SunBurst = EditorGUILayout.Toggle(
                        new GUIContent("Sunburst", "Is the sunburst effect enabled in this environment?"),
                        env.SunBurst);

                    if (env.SunBurst)
                    {
                        env.SunBurstColor = EditorGUILayout.ColorField(
                            new GUIContent("Sunburst color", "The color of the sunburst effect in this environment"),
                            env.SunBurstColor);
                    }
                }

                env.Clouds = EditorGUILayout.Toggle(new GUIContent("Clouds", "Are there clouds in this environment?"),
                    env.Clouds);
                if (env.Clouds)
                {
                    env.CloudColor = EditorGUILayout.ColorField(
                        new GUIContent("Cloud color", "The color of the clouds in this environment"), env.CloudColor);
                    env.NumberOfClouds = EditorGUILayout.IntField(
                        new GUIContent("Number of clouds", "The number of clouds to spawn in this environment"),
                        env.NumberOfClouds);
                    env.CloudMotion = EditorGUILayout.Toggle(
                        new GUIContent("Cloud motion", "Do the clouds in this environment move around?"),
                        env.CloudMotion);
                }

                env.SubtractiveFog =
                    EditorGUILayout.Toggle(
                        new GUIContent("Subtractive fog", "Is the fog in this environment additive or subtractive?"),
                        env.SubtractiveFog);

                EditorGUI.indentLevel--;
            }
        }
    }
}
