using UnityEngine;

namespace Torii.Console
{
    [CreateAssetMenu(menuName = "Torii/DevConsoleStyle")]
    public class UIDevConsoleStyle : ScriptableObject
    {
        public Color LogMessageColor = Color.white;
        public Color WarningMessageColor = Color.yellow;
        public Color ErrorMessageColor = Color.red;
        public Color ExceptionMessageColor = Color.red;

        public Sprite LogSprite;
        public Sprite WarningSprite;
        public Sprite ErrorSprite;
        public Sprite ExceptionSprite;
    }
}
