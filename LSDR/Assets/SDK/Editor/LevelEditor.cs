using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using libLSD.Formats;
using LSDR.Dream;
using LSDR.Entities;
using LSDR.IO;
using Torii.Pooling;
using Torii.Serialization;
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
        private readonly ToriiSerializer _serializer = new ToriiSerializer();
        private bool _showEntireMenu = true;
        private Vector2 _scrollPos;
        private List<Type> _entityTypes;
        private int _currentEntityType = 0;
        private bool _placingEntities = false;

        private const string ENTITY_NAMESPACE = "LSDR.Entities";

        [MenuItem("LSDR/Create level")]
        public static void Create()
        {
            LevelEditor editor = (LevelEditor)GetWindow(typeof(LevelEditor));
            editor.titleContent = new GUIContent("Level");
            editor.CenterOnMainWindow();

            editor._tilePool = AssetDatabase.LoadAssetAtPath<PrefabPool>("Assets/SDK/LBDTilePool.asset");
            editor._lbdReader = AssetDatabase.LoadAssetAtPath<LBDReaderSystem>("Assets/SDK/LBDReader.asset");

            editor._entityTypes = getClasses(ENTITY_NAMESPACE);

            // TODO: use existing level instead of overwriting...
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

        public void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            _showEntireMenu = EditorGUILayout.Foldout(_showEntireMenu, "LSDR Level Editor",
                new GUIStyle("foldout") {fontStyle = FontStyle.Bold});
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Import", GUILayout.Width(100)))
            {
                importDream();
            }
            if (GUILayout.Button("Export", GUILayout.Width(100)))
            {
                // TODO: exporting levels
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel++;

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            if (_showEntireMenu)
            {
                EditorGUILayout.LabelField("Entity placement", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                drawEntityPanel();
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndScrollView();

            EditorGUI.indentLevel--;
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

            if (CommonGUI.ColorButton(new GUIContent("Entity"), _placingEntities ? Color.green : Color.grey, GUILayout.Height(50)))
            {
                _placingEntities = !_placingEntities;
                Selection.activeGameObject = null;
            }

            if (_placingEntities)
            {
                handlePlaceEntity(sceneView);
            }

            GUILayout.EndVertical();

            GUILayout.EndArea();

            Handles.EndGUI();
            
            sceneView.Repaint();
        }

        private void handlePlaceEntity(SceneView sceneView)
        {
            var controlID = GUIUtility.GetControlID(GetHashCode(), FocusType.Passive);
            HandleUtility.AddDefaultControl(controlID);
            var curEvent = Event.current;

            if (curEvent.rawType == EventType.MouseDown && curEvent.button == 0)
            {
                Vector3 mousePos = curEvent.mousePosition;
                float ppp = EditorGUIUtility.pixelsPerPoint;
                mousePos.y = sceneView.camera.pixelHeight - mousePos.y * ppp;
                mousePos.x *= ppp;

                Ray ray = sceneView.camera.ScreenPointToRay(mousePos);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    createEntity(_entityTypes[_currentEntityType], hit.point);
                }
                Event.current.Use();
            }
        }

        private void createEntity(Type entityType, Vector3 pos)
        {
            GameObject entityObj = new GameObject(entityType.Name);
            entityObj.transform.position = pos;
            entityObj.transform.parent = _levelObj.transform;
            entityObj.AddComponent(entityType);
            Undo.RegisterCreatedObjectUndo(entityObj, entityObj.name);
            Selection.activeGameObject = entityObj;
        }

        private void drawEntityPanel()
        {
            var displayedOptions = _entityTypes.Select(t => new GUIContent(t.Name)).ToArray();
            _currentEntityType = EditorGUILayout.Popup(_currentEntityType, displayedOptions);
        }

        private void importDream()
        {
            string dreamPath = CommonGUI.BrowseForFile("Import dream...", new[] {"Dream JSON", "json"}, null);
            if (dreamPath == null) return;

            dreamPath = PathUtil.Combine(Application.streamingAssetsPath, dreamPath);

            Dream.Dream dream = _serializer.Deserialize<Dream.Dream>(dreamPath);

            if (dream.Type != DreamType.Legacy)
            {
                EditorUtility.DisplayDialog("Error importing dream", "Currently only legacy dreams are supported.",
                    "OK");
                return;
            }

            loadLBD(dream);
        }

        private void loadLBD(Dream.Dream dream)
        {
            // create a parent object for the LBD map -- destroy one if it already existed
            GameObject existingLBD = GameObject.Find("LBD");
            if (existingLBD != null)
            {
                DestroyImmediate(existingLBD);
            }
            GameObject lbdObj = new GameObject("LBD");
            
            // check to see if it already existed, and if not create it
            // TODO: delete existing and recreate if already existed
            GameObject tilePoolObj = GameObject.Find(_tilePool.Name);
            if (tilePoolObj == null)
            {
                _tilePool.Initialise();
            }
            _tilePool.ReturnAll();

            string lbdPath = PathUtil.Combine(Application.streamingAssetsPath, dream.Level);
            string[] lbdFiles = Directory.GetFiles(lbdPath, "*.LBD", SearchOption.AllDirectories);
            for (int i = 0; i < lbdFiles.Length; i++)
            {
                var lbdFile = lbdFiles[i];
                
                LBD lbd;
                using (BinaryReader br = new BinaryReader(File.Open(lbdFile, FileMode.Open)))
                {
                    lbd = new LBD(br);
                }

                GameObject tileMap = _lbdReader.CreateLBDTileMap(lbd, new Dictionary<TMDObject, Mesh>());
                
                // position the LBD 'slab' based on its tiling mode
                if (dream.LegacyTileMode == LegacyTileMode.Horizontal)
                {
                    int xPos = i % dream.TileWidth;
                    int yPos = i / dream.TileWidth;
                    int xMod = 0;
                    if (yPos % 2 == 1)
                    {
                        xMod = 10;
                    }
                    tileMap.transform.position = new Vector3((xPos * 20) - xMod, 0, yPos * 20);
                }
                
                tileMap.transform.SetParent(lbdObj.transform);
            }
            
            string tixFilePath = PathUtil.Combine(lbdPath, "TEXA.TIX");
            TIX tix;
            using (BinaryReader br = new BinaryReader(File.Open(tixFilePath, FileMode.Open)))
            {
                tix = new TIX(br);
            }

            _lbdReader.UseTIX(tix);
        }
        
        private static List<Type> getClasses(string ns)
        {
            var asm = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a =>
                a.GetName().Name == "Assembly-CSharp" ||
                a.GetName().Name == "LSDR.Common");
            if (asm == null)
            {
                Debug.LogError("Unable to find assembly for LSDR entities!");
                return null;
            }

            return asm.GetTypes()
                      .Where(type => type.IsClass && type.IsPublic && type.IsSubclassOf(typeof(MonoBehaviour)) &&
                                     type.Namespace != null && type.Namespace.StartsWith(ns)).ToList();
        }
    }
}
