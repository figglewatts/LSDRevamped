using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace LSDR.SDK.Editor.Windows
{
    public class TextureSetEditorWindow : EditorWindow
    {
        protected Texture2D[] _textureSetTextures;
        protected static readonly string[] _textureSetNames = {"Normal", "Kanji", "Downer", "Upper"};
        protected GUIStyle _textStyle;
        protected GUIStyle _textShadowStyle;
        protected GUIStyle _hoverTextStyle;
        protected GUIStyle _selectedTextStyle;
        protected bool _repaint;
        protected int _currentTextureSet = 0;

        protected static readonly int _textureSetPropertyID = Shader.PropertyToID("_TextureSet");

        [MenuItem("LSDR SDK/Show texture set window")]
        public static void Init()
        {
            TextureSetEditorWindow window = GetWindow<TextureSetEditorWindow>();
            window.titleContent = new GUIContent("Texture set");
            window.Show();
        }

        public void Awake()
        {
            _textureSetTextures = new Texture2D[4];
            for (int i = 0; i < 4; i++)
            {
                _textureSetTextures[i] = Resources.Load<Texture2D>($"tex{i}");
            }

            _textStyle = new GUIStyle(EditorStyles.boldLabel);
            _textStyle.normal.textColor = Color.white;
            _textStyle.fontSize = 32;
            _textStyle.alignment = TextAnchor.MiddleCenter;

            _textShadowStyle = new GUIStyle(_textStyle);
            _textShadowStyle.normal.textColor = Color.black;

            _hoverTextStyle = new GUIStyle(_textStyle);
            _hoverTextStyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f);

            _selectedTextStyle = new GUIStyle(_textStyle);
            _selectedTextStyle.normal.textColor = Color.yellow;

            _currentTextureSet = Shader.GetGlobalInt(_textureSetPropertyID);

            _repaint = true;
            repaintLoop();
        }

        public void OnDisable() { _repaint = false; }

        public void OnGUI()
        {
            for (int i = 0; i < 4; i++)
            {
                var rect = EditorGUILayout.GetControlRect(GUILayout.Height(60));
                var texture = _textureSetTextures[i];
                UnityEngine.GUI.DrawTextureWithTexCoords(rect, _textureSetTextures[i],
                    new Rect(0, 0, rect.width / texture.width * 0.5f,
                        rect.height / texture.height * 0.5f));
                var shadowRect = new Rect(rect.x + 2f, rect.y + 2f, rect.width - 2f, rect.height - 2f);
                EditorGUI.LabelField(shadowRect, _textureSetNames[i], _textShadowStyle);

                var style = _textStyle;
                if (rect.Contains(Event.current.mousePosition))
                {
                    style = _hoverTextStyle;

                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                    {
                        // button was pressed
                        setTextureSet(i);
                    }
                }

                EditorGUI.LabelField(rect, _textureSetNames[i], _currentTextureSet == i ? _selectedTextStyle : style);
            }
        }

        protected void setTextureSet(int index)
        {
            Shader.SetGlobalInt(_textureSetPropertyID, index + 1);
            _currentTextureSet = index;
        }

        protected async void repaintLoop()
        {
            while (_repaint)
            {
                Repaint();
                await Task.Delay(100);
            }
        }
    }
}
