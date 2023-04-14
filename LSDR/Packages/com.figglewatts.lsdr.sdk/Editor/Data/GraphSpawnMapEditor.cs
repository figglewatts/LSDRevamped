using LSDR.SDK.Data;
using LSDR.SDK.Editor.Util;
using LSDR.SDK.Editor.Windows;
using UnityEditor;
using UnityEngine;

namespace LSDR.SDK.Editor.Data
{
    [CustomEditor(typeof(GraphSpawnMap))]
    public class GraphSpawnMapEditor : UnityEditor.Editor
    {
        private const float GRAPH_GRID_OPACITY = 0.5f;
        private Vector2 _dreamBoxScrollPos = Vector2.zero;
        private Texture2D _graphTexture;
        private Rect _graphTextureRect;
        private Vector2 _graphTextureScaleFactor;

        private bool _needsRepaint;
        private int _selectedDream;
        private GUIStyle _selectedDreamStyle;
        private bool _showDreamBox = true;
        private GraphSpawnMap _graphSpawnMap => serializedObject.targetObject as GraphSpawnMap;

        private void OnEnable()
        {
            _graphTexture =
                AssetDatabase.LoadAssetAtPath<Texture2D>(
                    "Packages/com.figglewatts.lsdr.sdk/Editor/Assets/dreamGraph.png");

            _selectedDreamStyle = new GUIStyle
            {
                normal = new GUIStyleState
                {
                    background = TextureUtil.CreateColor(new Color(0, 0, 1, 0.1f))
                }
            };

            _graphTextureRect = new Rect(18, 4, 304, 304);

            _graphTextureScaleFactor = new Vector2(_graphTextureRect.width / _graphTexture.width,
                _graphTextureRect.height / _graphTexture.height);
        }

        public override void OnInspectorGUI()
        {
            if (_graphSpawnMap == null) return;

            serializedObject.Update();

            draw();
            handleInput();

            if (_needsRepaint)
            {
                _needsRepaint = false;
                Repaint();
                EditorUtility.SetDirty(_graphSpawnMap);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void draw()
        {
            drawGraph();
            drawDreamBox();
        }

        private void drawGraph()
        {
            Rect graphRect = GUILayoutUtility.GetRect(304, 304, 304, 304);
            GUI.BeginGroup(graphRect);
            {
                GUI.Box(new Rect(0, 0, 304, 304), GUIContent.none);
                GUI.DrawTexture(new Rect(0, 0, 304, 304), _graphTexture, ScaleMode.ScaleToFit);
                GUI.DrawTexture(new Rect(0, 0, 304, 304), _graphSpawnMap.GetTexture(GRAPH_GRID_OPACITY),
                    ScaleMode.ScaleToFit);
            }
            GUI.EndGroup();
        }

        private void drawDreamBox()
        {
            EditorGUILayout.BeginHorizontal();
            {
                _showDreamBox = EditorGUILayout.Foldout(_showDreamBox, "Dreams");
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(new GUIContent("Add from journal", "Add dreams from a journal")))
                {
                    if (_graphSpawnMap.Dreams.Count <= 0 || EditorUtility.DisplayDialog("Remove existing dreams?",
                            "Adding dreams from a journal will remove any existing dreams added. Are you sure?", "Yes",
                            "Cancel"))
                        GraphSpawnMapJournalDreamsEditorWindow.Show(_graphSpawnMap);
                }
            }
            EditorGUILayout.EndHorizontal();

            if (_showDreamBox && _graphSpawnMap.Dreams.Count > 0)
            {
                _dreamBoxScrollPos = EditorGUILayout.BeginScrollView(_dreamBoxScrollPos);
                {
                    for (int i = 0; i < _graphSpawnMap.Dreams.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal(_selectedDream == i ? _selectedDreamStyle : GUIStyle.none);
                        {
                            // dream display colour
                            Color lastCol = _graphSpawnMap.Dreams[i].Display;
                            Color newCol = EditorGUILayout.ColorField(GUIContent.none,
                                _graphSpawnMap.Dreams[i].Display, false, false, false,
                                GUILayout.Width(EditorGUIUtility.singleLineHeight));
                            if (newCol != lastCol)
                            {
                                _needsRepaint = true;
                                _graphSpawnMap.ModifyColor(i, newCol);
                            }

                            // the dream
                            _graphSpawnMap.Dreams[i].Dream = (Dream)EditorGUILayout.ObjectField(GUIContent.none,
                                _graphSpawnMap.Dreams[i].Dream, typeof(Dream), false);

                            // choose whether to paint with this dream or not
                            if (GUILayout.Button("Paint")) _selectedDream = i;

                            // remove this dream
                            if (GUILayout.Button("x"))
                            {
                                _graphSpawnMap.RemoveDream(i);
                                if (_selectedDream >= _graphSpawnMap.Dreams.Count)
                                    _selectedDream = _graphSpawnMap.Dreams.Count - 1;
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndScrollView();
            }
        }

        private void handleInput()
        {
            Event current = Event.current;
            if (_graphSpawnMap.Dreams.Count == 0 || !_graphTextureRect.Contains(current.mousePosition)) return;

            if (current.type == EventType.MouseDown && current.button == 0)
            {
                // paint on left click
                placeOnGrid(current.mousePosition, false);
            }
            else if (current.type == EventType.MouseDown && current.button == 1)
            {
                // clear on right click
                placeOnGrid(current.mousePosition, true);
            }
            else if (current.type == EventType.MouseDrag && current.button == 0 &&
                     _graphTextureRect.Contains(current.mousePosition + current.delta))
            {
                // paint on left click drag
                placeOnGrid(current.mousePosition + current.delta, false);
            }
            else if (current.type == EventType.MouseDrag && current.button == 1 &&
                     _graphTextureRect.Contains(current.mousePosition + current.delta))
            {
                // clear on right click drag
                placeOnGrid(current.mousePosition + current.delta, true);
            }
        }

        private void placeOnGrid(Vector2 mousePosition, bool clear)
        {
            Vector2 coordsInside = mousePosition - _graphTextureRect.position;
            Vector2Int gridCoords = new Vector2Int((int)(coordsInside.x / (64 * _graphTextureScaleFactor.x)),
                (int)(coordsInside.y / (64 * _graphTextureScaleFactor.y)));

            if (clear)
                _graphSpawnMap.ClearGraphSquare(gridCoords.x, GraphSpawnMap.GRAPH_SIZE - gridCoords.y - 1);
            else
            {
                _graphSpawnMap.SetGraphSquare(gridCoords.x, GraphSpawnMap.GRAPH_SIZE - gridCoords.y - 1,
                    _selectedDream);
            }

            _needsRepaint = true;
        }
    }
}
