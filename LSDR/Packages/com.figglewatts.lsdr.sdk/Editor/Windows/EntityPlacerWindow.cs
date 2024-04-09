using System;
using System.Collections.Generic;
using System.Reflection;
using LSDR.SDK.Entities;
using LSDR.SDK.Util;
using UnityEditor;
using UnityEngine;

namespace LSDR.SDK.Editor.Windows
{
    public class EntityPlacerWindow : EditorWindow
    {
        protected List<Type> _entityTypes = new List<Type>();
        protected Vector2 _scrollPos;
        protected Type _currentEntityType;
        protected bool _randomizeYRotation = false;
        protected GameObject _entitiesRoot;
        protected bool _placing = false;
        protected bool _autoName = false;

        [MenuItem("LSDR SDK/Entity Placer")]
        public static void Init()
        {
            EntityPlacerWindow window = GetWindow<EntityPlacerWindow>();
            window.titleContent = new GUIContent("Entity Placer");
            window.Show();
        }

        public void OnEnable()
        {
            _entityTypes = getEntityTypes();
            SceneView.duringSceneGui += onSceneGUI;
            UnityEditor.SceneManagement.PrefabStage.prefabStageOpened += onPrefabStageOpened;
            UnityEditor.SceneManagement.PrefabStage.prefabStageClosing += onPrefabStageClosing;
            findRootEntity();
        }

        public void OnDisable()
        {
            SceneView.duringSceneGui -= onSceneGUI;
            UnityEditor.SceneManagement.PrefabStage.prefabStageOpened -= onPrefabStageOpened;
            UnityEditor.SceneManagement.PrefabStage.prefabStageClosing -= onPrefabStageClosing;
        }

        public void OnGUI()
        {
            EditorGUILayout.LabelField("Click in the scene view to place");

            _randomizeYRotation = EditorGUILayout.ToggleLeft("Randomize Y rotation", _randomizeYRotation);
            _autoName = EditorGUILayout.ToggleLeft("Auto name entities", _autoName);
            _placing = EditorGUILayout.ToggleLeft("Placing entities", _placing);

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            {
                foreach (var entityType in _entityTypes)
                {
                    var lastBackground = GUI.backgroundColor;
                    if (entityType == _currentEntityType)
                    {
                        GUI.backgroundColor = new Color(lastBackground.r, lastBackground.g, 0.8f);
                    }

                    if (GUILayout.Button(entityType.Name))
                    {
                        _currentEntityType = entityType == _currentEntityType ? null : entityType;
                    }

                    GUI.backgroundColor = lastBackground;
                }
            }
            EditorGUILayout.EndScrollView();
        }

        protected void onPrefabStageOpened(UnityEditor.SceneManagement.PrefabStage stage)
        {
            findRootEntity();
        }

        protected void onPrefabStageClosing(UnityEditor.SceneManagement.PrefabStage stage)
        {
            findRootEntity();
        }

        protected void findRootEntity()
        {
            var prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage == null)
            {
                _entitiesRoot = GameObject.Find("Entities");
                if (_entitiesRoot == null) _entitiesRoot = new GameObject("Entities");
            }
            else
            {
                _entitiesRoot = prefabStage.prefabContentsRoot.transform.Find("Entities").gameObject;
            }
        }

        protected List<Type> getEntityTypes()
        {
            List<Type> types = new List<Type>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsSubclassOf(typeof(BaseEntity)))
                    {
                        types.Add(type);
                    }
                }
            }

            return types;
        }

        protected void onSceneGUI(SceneView sceneView)
        {
            if (!_placing) return;

            int controlID = GUIUtility.GetControlID(FocusType.Passive);

            Event e = Event.current;

            if (e.button != 0) return;
            if (e.type != EventType.MouseDown && e.type != EventType.MouseUp) return;

            if (e.type == EventType.MouseDown)
            {
                GUIUtility.hotControl = controlID;

                Ray worldRay = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                if (Physics.Raycast(worldRay, out RaycastHit hitInfo))
                {
                    placeEntity(hitInfo.point);
                }
            }
            else if (e.type == EventType.MouseUp)
            {
                GUIUtility.hotControl = 0;
            }

            e.Use();
        }

        protected void placeEntity(Vector3 worldPosition)
        {
            if (_currentEntityType == null) return;

            GameObject entityObj = new GameObject(_currentEntityType.Name);
            entityObj.transform.SetParent(_entitiesRoot.transform);
            entityObj.AddComponent(_currentEntityType);
            if (_autoName)
            {
                entityObj.name = $"{_currentEntityType.Name}_$$";
            }
            else
            {
                entityObj.name = $"{_currentEntityType.Name}_{entityObj.name}";
            }
            entityObj.GetComponent<BaseEntity>().ID = entityObj.name;

            var iconContent = EditorGUIUtility.IconContent("sv_label_1");
            EditorGUIUtility.SetIconForObject(entityObj, (Texture2D)iconContent.image);

            if (_randomizeYRotation)
            {
                float degrees = RandUtil.Float(0, 360);
                entityObj.transform.Rotate(Vector3.up, degrees);
            }
            entityObj.transform.position = worldPosition;

            Undo.RegisterCreatedObjectUndo(entityObj, $"{_currentEntityType.Name} placed with Entity Placer");
        }
    }
}
