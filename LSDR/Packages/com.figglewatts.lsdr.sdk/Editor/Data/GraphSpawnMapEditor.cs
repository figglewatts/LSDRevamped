using System;
using System.IO;
using System.Linq;
using LSDR.SDK.Data;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LSDR.SDK
{
    public class GraphSpawnMapEditor : EditorWindow
    {
        public GraphSpawnMap GraphSpawnMap { get => _graphSpawnMap; set => _graphSpawnMap = value; }

        private GraphSpawnMap _graphSpawnMap;

        private bool _needsRepaint;
        private Texture2D _graphTexture;
        private Texture2D _gridTex;

        private const int _margin = 2;
        private Rect _graphTextureRect = new Rect(0, 0, 304, 304);
        private Vector2 _graphTextureScaleFactor;
        private Rect _dreamBoxRect;
        private Vector2 _dreamBoxScrollPos = Vector2.zero;
        private Rect _bottomRect;

        private int _selectedDream = 0;

        private const float GRAPH_GRID_OPACITY = 0.5f;
        private const int WINDOW_WIDTH = 650;

        private void OnGUI()
        {
            if (_graphSpawnMap == null) return;

            draw();
            handleInput();

            if (_needsRepaint)
            {
                _needsRepaint = false;
                Repaint();
            }
        }

        private void draw()
        {
            drawGraph();
            drawDreamBox();
        }

        private void drawGraph()
        {
            GUI.Box(_graphTextureRect, "");
            GUI.DrawTexture(_graphTextureRect, _graphTexture);
            GUI.DrawTexture(_graphTextureRect, GraphSpawnMap.GetTexture(GRAPH_GRID_OPACITY));
        }

        private void drawDreamBox()
        {
            float colorSize = EditorGUIUtility.singleLineHeight;
            float xPos = _graphTextureRect.width + _margin;
            float toolbarHeight = 18;
            _dreamBoxRect = new Rect(xPos, _graphTextureRect.y + toolbarHeight, position.width - xPos,
                position.height - toolbarHeight);

            // toolbar
            float toolbarWidth = position.width - _graphTextureRect.width;
            GUI.BeginGroup(new Rect(_dreamBoxRect.x - _margin, 0, toolbarWidth, toolbarHeight), EditorStyles.toolbar);
            if (GUI.Button(new Rect(0, 0, 40, toolbarHeight), "Apply", EditorStyles.toolbarButton))
            {
                Close();
            }

            float btnWidth = 18;
            if (GUI.Button(new Rect(toolbarWidth - btnWidth, 0, btnWidth, toolbarHeight), "-",
                EditorStyles.toolbarButton))
            {
                removeSelectedDream();
            }

            if (GUI.Button(new Rect(toolbarWidth - btnWidth * 2, 0, btnWidth, toolbarHeight), "+",
                EditorStyles.toolbarButton))
            {
                addNewDream();
            }

            GUI.EndGroup();

            Rect dreamBoxContentRect = new Rect(0, 0, _dreamBoxRect.width - 30,
                GraphSpawnMap.Dreams.Count * (EditorGUIUtility.singleLineHeight + 4));
            _dreamBoxScrollPos = GUI.BeginScrollView(_dreamBoxRect, _dreamBoxScrollPos, dreamBoxContentRect);

            if (GraphSpawnMap.Dreams.Count > 0)
            {
                _selectedDream = ListBox.Draw(
                    new Rect(colorSize + _margin, 0, position.width - xPos - _margin, dreamBoxContentRect.height),
                    _selectedDream,
                    GraphSpawnMap.Dreams.Select(d => Path.GetFileName(d.Path)).ToArray());

                for (int i = 0; i < GraphSpawnMap.Dreams.Count; i++)
                {
                    Rect pos = new Rect(0,
                        2 + (EditorGUIUtility.singleLineHeight + 4) * i,
                        EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);
                    var lastCol = GraphSpawnMap.Dreams[i].Display;
                    var newCol = EditorGUI.ColorField(pos, GUIContent.none,
                        GraphSpawnMap.Dreams[i].Display, false, false, false, null);
                    if (newCol != lastCol)
                    {
                        _needsRepaint = true;
                        GraphSpawnMap.ModifyColor(i, newCol);
                    }
                }
            }

            GUI.EndScrollView();
        }

        private void handleInput()
        {
            if (GraphSpawnMap.Dreams.Count == 0) return;

            var current = Event.current;

            if (current.type == EventType.MouseDown && current.button == 0 &&
                _graphTextureRect.Contains(current.mousePosition))
            {
                placeOnGrid(current.mousePosition);
            }
            else if (current.type == EventType.MouseDrag && current.button == 0 &&
                     _graphTextureRect.Contains(current.mousePosition + current.delta))
            {
                placeOnGrid(current.mousePosition + current.delta);
            }
        }

        private void placeOnGrid(Vector2 mousePosition)
        {
            Vector2 coordsInside = mousePosition - _graphTextureRect.position;
            Vector2Int gridCoords = new Vector2Int((int)(coordsInside.x / (64 * _graphTextureScaleFactor.x)),
                (int)(coordsInside.y / (64 * _graphTextureScaleFactor.y)));
            GraphSpawnMap.Set(gridCoords.x, GraphSpawnMap.GRAPH_SIZE - gridCoords.y - 1, _selectedDream);
            _needsRepaint = true;
        }

        private void addNewDream()
        {
            var dreamPath =
                EditorUtility.OpenFilePanelWithFilters("Open dream JSON...", "", new[] {"Dream JSON file", "json"});

            if (!string.IsNullOrEmpty(dreamPath))
            {
                // now remove everything before StreamingAssets path
                var indexOf = dreamPath.IndexOf("StreamingAssets", StringComparison.Ordinal) + "StreamingAssets".Length;
                dreamPath = dreamPath.Substring(indexOf);

                Color displayCol = new Color(Random.value, Random.value, Random.value, 1);
                GraphSpawnMap.Add(dreamPath, displayCol);
                _selectedDream = GraphSpawnMap.Dreams.Count - 1;
            }
        }

        private void removeSelectedDream()
        {
            GraphSpawnMap.Remove(_selectedDream);

            if (_selectedDream >= GraphSpawnMap.Dreams.Count)
            {
                _selectedDream = GraphSpawnMap.Dreams.Count - 1;
            }
        }

        private void OnEnable()
        {
            titleContent.text = "Graph Editor";
            _graphTexture =
                AssetDatabase.LoadAssetAtPath<Texture2D>(
                    "Packages/com.figglewatts.lsdr.sdk/Editor/Assets/dreamGraph.png");

            _graphTextureScaleFactor = new Vector2(_graphTextureRect.width / _graphTexture.width,
                _graphTextureRect.height / _graphTexture.height);

            minSize = new Vector2(WINDOW_WIDTH, _graphTextureRect.height);
            maxSize = minSize;
        }
    }
}
