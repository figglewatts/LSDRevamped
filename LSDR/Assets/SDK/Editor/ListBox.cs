using Torii.Util;
using UnityEditor;
using UnityEngine;

namespace LSDR.SDK
{
    public static class ListBox
    {
        private static readonly GUIStyle _style;
        private static readonly GUIStyle _boxStyle;

        static ListBox()
        {
            Color textColor = EditorGUIUtility.isProSkin ? new Color(0.7f, 0.7f, 0.7f, 1.0f) : Color.black;
            
            GUIStyleState normal = new GUIStyleState()
            {
                textColor = textColor
            };
            
            GUIStyleState selected = new GUIStyleState()
            {
                textColor = Color.white,
                background = TextureUtil.CreateColor(new Color(0.349f, 0.537f, 0.812f))
            };
            
            _style = new GUIStyle()
            {
                normal = normal,
                hover = normal,
                active = normal,
                focused = selected,
                onNormal = selected,
                onFocused = selected,
                onActive = selected,
                margin = new RectOffset(0, 0, 0, 0),
                padding = new RectOffset(6, 6, 6, 6),
                alignment = TextAnchor.MiddleLeft
            };

            Texture2D boxSprite = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/SDK/box-sprite.png");
            _boxStyle = new GUIStyle()
            {
                normal = new GUIStyleState()
                {
                    background = boxSprite
                },
                border = new RectOffset(4, 4, 4, 4)
            };
        }

        public static int Draw(Rect rect, int selected, string[] contents)
        {
            GUI.Box(rect, "", _boxStyle);
            float gridHeight = (EditorGUIUtility.singleLineHeight + 4) * contents.Length;
            return GUI.SelectionGrid(new Rect(rect.x, rect.y, rect.width, gridHeight), selected, contents, 1, _style);
        }
    }
}
