using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LSDR.SDK.Editor.Windows
{
    public class TextureUVRectHelper : EditorWindow
    {
        [SerializeField] protected Rect _rect;
        [SerializeField] protected Rect _result;
        [SerializeField] protected Texture2D _tex;
        protected Texture2D _visualisation;
        protected Vector2 _scrollPos;

        public void OnGUI()
        {
            SerializedObject target = new SerializedObject(this);
            target.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginVertical();
            {
                _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
                {
                    drawControls(target);
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();

            target.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
            {
                onControlsChanged();
            }
        }

        protected void drawControls(SerializedObject target)
        {
            EditorGUILayout.PropertyField(target.FindProperty("_tex"), new GUIContent("Texture"));

            if (_tex == null)
            {
                EditorGUILayout.HelpBox("To use this tool you need to select a texture", MessageType.Warning,
                    wide: true);
            }

            EditorGUI.BeginDisabledGroup(_tex == null);
            {
                EditorGUILayout.PropertyField(target.FindProperty("_rect"), new GUIContent("Subrect (in pixels)"));

                EditorGUI.BeginDisabledGroup(true);
                {
                    EditorGUILayout.PropertyField(target.FindProperty("_result"), new GUIContent("Result (in UVs)"));
                }
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.LabelField("(use result values with 'Right Click>Copy')");

                GUILayout.FlexibleSpace();
            }
            EditorGUI.EndDisabledGroup();

            drawTexture();
        }

        protected void onControlsChanged()
        {
            recalculate();
            regenerateVisualisationTexture();
        }

        protected void regenerateVisualisationTexture()
        {
            if (_tex == null) return;
            if (_visualisation != null) DestroyImmediate(_visualisation);

            Debug.Log("regenerating viz");
            _visualisation = new Texture2D(_tex.width, _tex.height, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point
            };
            Color[] clearPixels = Enumerable.Repeat(Color.clear, _tex.width * _tex.height).ToArray();
            _visualisation.SetPixels(clearPixels);

            int yStart = (int)_rect.y;
            int xStart = (int)_rect.x;
            for (int y = yStart; y < yStart + _rect.height; y++)
            {
                for (int x = xStart; x < xStart + _rect.width; x++)
                {
                    _visualisation.SetPixel(x, _tex.height - y, new Color(0, 1, 0, 0.5f));
                }
            }
            _visualisation.Apply();
        }

        protected void recalculate()
        {
            if (_tex == null) return;

            _result.size = _rect.size / new Vector2Int(_tex.width, _tex.height);
            _result.position = (_rect.position / new Vector2Int(_tex.width, _tex.height));

            // flip Y coord and subtract height for pos as (0,0) in uv-space is bottom left not top left
            _result.position = new Vector2(2 * _result.position.x, 1) - _result.position -
                               new Vector2(0, _result.size.y);
        }

        protected void drawTexture()
        {
            if (_tex == null) return;

            Rect textureRect = GUILayoutUtility.GetAspectRect(_tex.width / (float)_tex.height);
            GUI.BeginGroup(textureRect);
            {
                var groupRect = new Rect(0, 0, textureRect.width, textureRect.height);
                GUI.DrawTexture(groupRect, _tex, ScaleMode.ScaleToFit);
                if (_visualisation != null) GUI.DrawTexture(groupRect, _visualisation, ScaleMode.ScaleToFit);
            }
            GUI.EndGroup();
        }

        [MenuItem("LSDR SDK/Texture UV Rect Helper")]
        public static void Init()
        {
            TextureUVRectHelper window = GetWindow<TextureUVRectHelper>();
            window.titleContent = new GUIContent("Texture UV Rect Helper");
            window.Show();
        }
    }
}
