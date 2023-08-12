using System;

namespace Torii.UnityEditor
{
    [Serializable]
    public class ScenePicker
    {
        public string ScenePath;

        public static implicit operator string(ScenePicker scenePicker)
        {
            if (scenePicker == null)
            {
                return string.Empty;
            }

            return scenePicker.ScenePath;
        }
    }
}
