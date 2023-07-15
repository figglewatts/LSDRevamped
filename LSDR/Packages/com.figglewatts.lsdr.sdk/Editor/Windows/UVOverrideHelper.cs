using UnityEditor;
using UnityEngine;

namespace LSDR.SDK.Editor.Windows
{
    public class UVOverrideHelper : EditorWindow
    {
        [SerializeField] protected Rect _rect;
        [SerializeField] protected Rect _result;
        protected Vector2 _scrollPos;

        protected const int TEX_WIDTH = 2056;
        protected const int TEX_HEIGHT = 512;

        public void OnGUI()
        {
            SerializedObject target = new SerializedObject(this);

            EditorGUILayout.BeginVertical();
            {
                _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
                {
                    EditorGUILayout.PropertyField(target.FindProperty("_rect"));
                    recalculate();

                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.PropertyField(target.FindProperty("_result"));
                    EditorGUI.EndDisabledGroup();

                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();

            target.ApplyModifiedProperties();
        }

        protected void recalculate()
        {
            _result.size = _rect.size / new Vector2Int(TEX_WIDTH, TEX_HEIGHT);
            _result.position = (_rect.position / new Vector2Int(TEX_WIDTH, TEX_HEIGHT));

            // flip Y coord and subtract height for pos as (0,0) in uv-space is bottom left not top left
            _result.position = new Vector2(2 * _result.position.x, 1) - _result.position -
                               new Vector2(0, _result.size.y);
        }

        [MenuItem("LSDR SDK/UV Override Helper")]
        public static void Init()
        {
            UVOverrideHelper window = GetWindow<UVOverrideHelper>();
            window.titleContent = new GUIContent("UV Override Helper");
            window.Show();
        }
    }
}
