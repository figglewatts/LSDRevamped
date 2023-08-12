using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LSDR.SDK.Editor.Windows
{
    public class LBDArrangerEditorWindow : EditorWindow
    {
        public enum LBDLayoutType
        {
            Horizontal,
            Vertical
        }

        public List<Transform> ArrangedLBDs = new List<Transform>();
        public LBDLayoutType LayoutType;
        public Vector2Int LBDDimensions = new Vector2Int(x: 20, y: 20);
        public int TileWidth;

        protected Vector2 _scrollPos;

        public void OnGUI()
        {
            SerializedObject target = new SerializedObject(this);

            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.PropertyField(target.FindProperty("LayoutType"), new GUIContent("Layout"));
                EditorGUILayout.PropertyField(target.FindProperty("LBDDimensions"), new GUIContent("LBD dimensions"));

                if (LayoutType == LBDLayoutType.Horizontal)
                    TileWidth = EditorGUILayout.IntSlider(new GUIContent("Tile dimension"), TileWidth, leftValue: 0,
                        rightValue: 16);

                _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
                {
                    EditorGUILayout.PropertyField(target.FindProperty("ArrangedLBDs"), new GUIContent("LBDs"));

                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.EndScrollView();

                if (GUILayout.Button("Arrange")) arrangeLBDs();
            }
            EditorGUILayout.EndVertical();

            target.ApplyModifiedProperties();
        }

        protected void arrangeLBDs()
        {
            for (int i = 0; i < ArrangedLBDs.Count; i++)
            {
                Transform transform = ArrangedLBDs[i];
                Vector3 lbdPos = Vector3.zero;
                if (LayoutType == LBDLayoutType.Horizontal)
                {
                    int xPos = i % TileWidth;
                    int yPos = i / TileWidth;
                    int xMod = 0;
                    if (yPos % 2 == 1) xMod = 10;

                    lbdPos = new Vector3(xPos * LBDDimensions.x - xMod, y: 0, yPos * LBDDimensions.y);
                }

                transform.position = lbdPos;
            }
        }

        [MenuItem("LSDR SDK/LBD arranger")]
        public static void Init()
        {
            LBDArrangerEditorWindow window = GetWindow<LBDArrangerEditorWindow>();
            window.titleContent = new GUIContent("LBD arranger");
            window.Show();
        }
    }
}
