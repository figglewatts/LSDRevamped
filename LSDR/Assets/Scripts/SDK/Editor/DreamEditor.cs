using System;
using System.Collections.Generic;
using System.Net.Configuration;
using UnityEditor;
using LSDR.Dream;
using Torii.Serialization;
using Torii.Util;
using UnityEngine;

namespace LSDR.SDK
{
    public class DreamEditor : EditorWindow
    {
        // TODO: import dreams
        
        // TODO: environment preview
        
        private Dream.Dream _dream;
        private Vector2 _scrollPos;
        private readonly ToriiSerializer _serializer = new ToriiSerializer();
        private readonly List<bool> _dreamEnvironmentFoldoutStates = new List<bool>();
        private readonly Stack<int> _dreamEnvironmentsToRemove = new Stack<int>();
        
        [MenuItem("LSDR/Create dream")]
        public static void Init()
        {
            DreamEditor editor = (DreamEditor)EditorWindow.GetWindow(typeof(DreamEditor));
            editor.titleContent = new GUIContent("Dream");
            editor.CenterOnMainWindow();
            editor._dream = new Dream.Dream {Type = DreamType.Revamped};
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
            EditorGUILayout.BeginHorizontal();
            _dream.Level = EditorGUILayout.TextField(
                new GUIContent("Level",
                    "The path to the raw level file within the game's data to load for this dream. " +
                    "Can be an LBD directory or a TMAP."),
                _dream.Level);
            if (GUILayout.Button("Browse"))
            {
                _dream.Level = browseForDreamLevelFile();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Gameplay", EditorStyles.boldLabel);
            _dream.GreyMan =
                EditorGUILayout.Toggle(new GUIContent("Grey man", "Whether or not this dream can spawn the grey man"),
                    _dream.GreyMan);
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

        private string browseForDreamLevelFile()
        {
            GUI.FocusControl(null);
            string levelPath;
            if (_dream.Type == DreamType.Legacy)
            {
                levelPath = EditorUtility.OpenFolderPanel("Open LBD directory...", "", "");
            }
            else
            {
                levelPath = EditorUtility.OpenFilePanelWithFilters("Open TMAP file...", "", new [] {"Torii map files", "tmap"});
            }

            if (string.IsNullOrEmpty(levelPath))
            {
                return _dream.Level;
            }

            if (!levelPath.Contains("StreamingAssets"))
            {
                EditorUtility.DisplayDialog("Level error", "Your level must be in the 'StreamingAssets' directory!",
                    "Ok");
                return _dream.Level;
            }
            
            // now remove everything before StreamingAssets path
            var indexOf = levelPath.IndexOf("StreamingAssets", StringComparison.Ordinal) + "StreamingAssets".Length;
            return levelPath.Substring(indexOf);
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

                EditorGUI.indentLevel--;
            }
        }
    }
}
