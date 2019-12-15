using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using libLSD.Formats;
using LSDR.Dream;
using LSDR.Entities;
using LSDR.Game;
using LSDR.IO;
using LSDR.IO.ResourceHandlers;
using Torii.Pooling;
using Torii.Serialization;
using Torii.UnityEditor;
using Torii.Util;
using UnityEditor;
using UnityEngine;
using ResourceManager = Torii.Resource.ResourceManager;

namespace LSDR.SDK
{
    public class LevelEditor : EditorWindow
    {
        private const int POS_PADDING = 5;
        private const int CONTROLS_WIDTH = 50;
        private GameObject _levelObj;
        private LBDFastMeshSystem _lbdReader;
        private LevelLoaderSystem _levelLoader;
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

            editor._lbdReader = AssetDatabase.LoadAssetAtPath<LBDFastMeshSystem>("Assets/SDK/LBDFastMesh.asset");
            editor._levelLoader = AssetDatabase.LoadAssetAtPath<LevelLoaderSystem>("Assets/SDK/LevelLoader.asset");

            editor._entityTypes = getClasses(ENTITY_NAMESPACE);
            
            GameObject existingLevel = GameObject.Find("Level");
            if (existingLevel != null)
            {
                editor._levelObj = existingLevel;
            }

            editor.Show();
        }

        public void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            _showEntireMenu = EditorGUILayout.Foldout(_showEntireMenu, "LSDR Level Editor",
                new GUIStyle("foldout") {fontStyle = FontStyle.Bold});
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("New", GUILayout.Width(100)))
            {
                newLevel();
            }
            if (GUILayout.Button("Import", GUILayout.Width(100)))
            {
                importLevel();
            }
            if (GUILayout.Button("Export", GUILayout.Width(100)))
            {
                exportLevel();
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

        private void OnEnable()
        {
            SceneView.onSceneGUIDelegate += OnSceneGUI;
            ResourceManager.RegisterHandler(new LBDHandler());
            ResourceManager.RegisterHandler(new TIXHandler());
        }

        public void OnSceneGUI(SceneView sceneView)
        {
            if (_levelObj == null) return;
            
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

            Rect bottomArea = new Rect(POS_PADDING, sceneView.position.height - 50 - (POS_PADDING * 4),
                sceneView.position.width - (POS_PADDING * 2), 50);
            GUILayout.BeginArea(bottomArea);
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (SnapToGrid.Enabled)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("-")) SnapToGrid.Resolution /= 2;
                GUILayout.Label(SnapToGrid.Resolution.ToString(CultureInfo.InvariantCulture));
                if (GUILayout.Button("+")) SnapToGrid.Resolution *= 2;
                GUILayout.EndHorizontal();
            }
            SnapToGrid.Enabled = GUILayout.Toggle(SnapToGrid.Enabled, "Snap to grid");
            
            GUILayout.EndHorizontal();
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
            entityObj.AddComponent<SnapToGrid>();
            Undo.RegisterCreatedObjectUndo(entityObj, entityObj.name);
            Selection.activeGameObject = entityObj;
        }

        private void drawEntityPanel()
        {
            var displayedOptions = _entityTypes.Select(t => new GUIContent(t.Name)).ToArray();
            _currentEntityType = EditorGUILayout.Popup(_currentEntityType, displayedOptions);
        }

        private void newLevel()
        {
            // check to see if there was an existing level, as we'll have to overwrite it
            GameObject existingLevel = GameObject.Find("Level");
            if (existingLevel)
            {
                bool yes = EditorUtility.DisplayDialog("Existing level",
                    "There is an existing level loaded. Are you sure you " +
                    "want to import another level? Any unsaved changes will " +
                    "be lost.", "Yes", "Cancel");
                if (!yes) return;
                
                DestroyImmediate(existingLevel);
            }
            
            GameObject levelObj = new GameObject("Level");
            Selection.activeGameObject = levelObj;
            _levelObj = levelObj;
        }

        private void exportLevel()
        {
            var path = EditorUtility.SaveFilePanel("Export level", "", ".tmap", "tmap");

            if (!string.IsNullOrEmpty(path))
            {
                Level level = Level.FromScene(_levelObj);
                _serializer.Serialize(level, path);
            }
        }

        private void importLevel()
        {
            // check to see if there was an existing level, as we'll have to overwrite it
            GameObject existingLevel = GameObject.Find("Level");
            if (existingLevel)
            {
                bool yes = EditorUtility.DisplayDialog("Existing level",
                    "There is an existing level loaded. Are you sure you " +
                    "want to import another level? Any unsaved changes will " +
                    "be lost.", "Yes", "Cancel");
                if (!yes) return;
            }
            
            string levelPath = CommonGUI.BrowseForFile("Import level...",
                new[] {"Level files", "tmap,json"}, null);
            if (levelPath == null) return;
            
            DestroyImmediate(existingLevel);

            levelPath = PathUtil.Combine(Application.streamingAssetsPath, levelPath);

            if (Path.GetExtension(levelPath) == ".json")
            {
                Dream.Dream dream = _serializer.Deserialize<Dream.Dream>(levelPath);
                if (dream.Type == DreamType.Legacy)
                {
                    loadLBD(dream);
                }

                if (!string.IsNullOrEmpty(dream.Level))
                {
                    string tmapPath = PathUtil.Combine(Application.streamingAssetsPath, dream.Level);
                    _levelObj = _levelLoader.LoadLevel(tmapPath);
                }
                else
                {
                    newLevel();
                }
                
            }
            else
            {
                _levelObj = _levelLoader.LoadLevel(levelPath);
            }
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

            string lbdPath = PathUtil.Combine(Application.streamingAssetsPath, dream.LBDFolder);
            _lbdReader.LoadLBD(lbdPath, dream.LegacyTileMode, dream.TileWidth);
            
            string tixFilePath = PathUtil.Combine(lbdPath, "TEXA.TIX");
            _lbdReader.UseTIX(tixFilePath);
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
                      .Where(type => !AttributeUtil.HasAttribute<EntityExcludeAttribute>(type) && type.IsClass &&
                                     type.IsPublic && type.IsSubclassOf(typeof(BaseEntity)) &&
                                     type.Namespace != null && type.Namespace != ns && type.Namespace.StartsWith(ns))
                      .ToList();
        }
    }
}
