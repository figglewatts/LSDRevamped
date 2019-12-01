using System;
using System.Collections.Generic;
using System.IO;
using libLSD.Formats;
using LSDR.Dream;
using LSDR.Entities;
using LSDR.IO;
using Torii.Pooling;
using Torii.Util;
using UnityEditor;
using UnityEngine;

namespace LSDR.SDK
{
    public class LevelEditor : EditorWindow
    {
        private const int POS_PADDING = 5;
        private const int CONTROLS_WIDTH = 50;
        private GameObject _levelObj;
        private PrefabPool _tilePool;
        private LBDReaderSystem _lbdReader;

        [MenuItem("LSDR/Create level")]
        public static void Create()
        {
            LevelEditor editor = (LevelEditor)GetWindow(typeof(LevelEditor));
            editor.titleContent = new GUIContent("Level");
            editor.CenterOnMainWindow();

            editor._tilePool = AssetDatabase.LoadAssetAtPath<PrefabPool>("Assets/SDK/LBDTilePool.asset");
            editor._lbdReader = AssetDatabase.LoadAssetAtPath<LBDReaderSystem>("Assets/SDK/LBDReader.asset");

            GameObject tilePoolObj = GameObject.Find(editor._tilePool.Name);
            if (tilePoolObj != null)
            {
                editor._tilePool.ReturnAll();
            }
            else
            {
                editor._tilePool.Initialise();
            }
            
            GameObject existingLevel = GameObject.Find("Level");
            if (existingLevel != null)
            {
                // check to see if the user is cool with us destroying their existing level
                bool response = EditorUtility.DisplayDialog("New level error",
                    "Level already exists in scene, are you sure you want to create a new one? " +
                    "The existing level will be lost if it's unsaved.", "Yes", "Cancel");
                if (!response)
                {
                    // they said it's not cool -- abort!
                    editor.Close();
                    return;
                }
                
                // yeet
                DestroyImmediate(existingLevel);
            }
            
            GameObject levelObj = new GameObject("Level");
            Level level = levelObj.AddComponent<Level>();
            Selection.activeGameObject = levelObj;
            editor._levelObj = levelObj;
            editor.Show();
        }

        private void OnDisable() { SceneView.onSceneGUIDelegate -= OnSceneGUI; }
        private void OnEnable() { SceneView.onSceneGUIDelegate += OnSceneGUI; }

        public void OnSceneGUI(SceneView sceneView)
        {
            Handles.BeginGUI();

            Rect controlsArea = new Rect(POS_PADDING, POS_PADDING, 
                CONTROLS_WIDTH, sceneView.position.height - (POS_PADDING * 10));

            GUILayout.BeginArea(controlsArea);

            GUILayout.BeginVertical();

            if (GUILayout.Button("LBD", GUILayout.Height(CONTROLS_WIDTH)))
            {
                loadLBD();
            }

            GUILayout.EndVertical();

            GUILayout.EndArea();

            Handles.EndGUI();
        }

        private void loadLBD()
        {
            string lbdPath = CommonGUI.BrowseForFile("Import LBD file...", new[] {"LBD file", "lbd"}, null);
            lbdPath = PathUtil.Combine(Application.streamingAssetsPath, lbdPath);

            if (lbdPath != null)
            {
                LBD lbd;
                using (BinaryReader br = new BinaryReader(File.Open(lbdPath, FileMode.Open)))
                {
                    lbd = new LBD(br);
                }

                _lbdReader.CreateLBDTileMap(lbd, new Dictionary<TMDObject, Mesh>());

                string tixFilePath = PathUtil.Combine(Path.GetDirectoryName(lbdPath), "TEXA.TIX");
                TIX tix;
                using (BinaryReader br = new BinaryReader(File.Open(tixFilePath, FileMode.Open)))
                {
                    tix = new TIX(br);
                }
                _lbdReader.UseTIX(tix);
            }
        }
    }
}
