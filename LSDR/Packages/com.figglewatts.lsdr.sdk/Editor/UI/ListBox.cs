using LSDR.SDK.Editor.Util;
using UnityEditor;
using UnityEngine;

namespace LSDR.SDK.Editor.UI
{
    public static class ListBox
    {
        private static readonly GUIStyle _style;
        private static readonly GUIStyle _boxStyle;

        static ListBox()
        {
            Color textColor = EditorGUIUtility.isProSkin ? new Color(r: 0.7f, g: 0.7f, b: 0.7f, a: 1.0f) : Color.black;

            GUIStyleState normal = new GUIStyleState
            {
                textColor = textColor
            };

            GUIStyleState selected = new GUIStyleState
            {
                textColor = Color.white,
                background = TextureUtil.CreateColor(new Color(r: 0.349f, g: 0.537f, b: 0.812f))
            };

            _style = new GUIStyle
            {
                normal = normal,
                hover = normal,
                active = normal,
                focused = selected,
                onNormal = selected,
                onFocused = selected,
                onActive = selected,
                margin = new RectOffset(left: 0, right: 0, top: 0, bottom: 0),
                padding = new RectOffset(left: 6, right: 6, top: 6, bottom: 6),
                alignment = TextAnchor.MiddleLeft
            };

            Texture2D boxSprite =
                AssetDatabase.LoadAssetAtPath<Texture2D>(
                    "Packages/com.figglewatts.lsdr.sdk/Assets/box-sprite.png");
            _boxStyle = new GUIStyle
            {
                normal = new GUIStyleState
                {
                    background = boxSprite
                },
                border = new RectOffset(left: 4, right: 4, top: 4, bottom: 4)
            };
        }

        public static int Draw(Rect rect, int selected, string[] contents)
        {
            GUI.Box(rect, "", _boxStyle);
            float gridHeight = (EditorGUIUtility.singleLineHeight + 4) * contents.Length;
            return GUI.SelectionGrid(new Rect(rect.x, rect.y, rect.width, gridHeight), selected, contents, xCount: 1,
                _style);
        }
    }
}
