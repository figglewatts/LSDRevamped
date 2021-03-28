using UnityEngine;

namespace LSDR.SDK.Editor.GUI
{
    public static class ToggleButton
    {
        public static bool OnGUI(bool value, GUIContent content, params GUILayoutOption[] options)
        {
            return GUILayout.Toggle(value, content, "Button", options) != value;
        }
    }
}
