using System;
using UnityEngine;

namespace Torii.UI
{
    [Serializable]
    public class ScenePicker
    {
        public string ScenePath;

        public static implicit operator string(ScenePicker scenePicker)
        {
            if (scenePicker == null)
            {
                return String.Empty;
            }
            
            return scenePicker.ScenePath;
        }
    }
}
