using LSDR.Dream;
using UnityEditor;
using UnityEngine;

namespace LSDR.SDK
{
    public static class GraphSpawnMapDrawer
    {
        public static void OnGUI(Rect position, GraphSpawnMap spawnMap, GUIContent label)
        {
            Event guiEvent = Event.current;
            float labelWidth = GUI.skin.label.CalcSize(label).x + 10;
            Rect buttonRect = new Rect(position.x + labelWidth, position.y, position.width - labelWidth,
                position.height);

            if (guiEvent.type == EventType.Repaint)
            {
                GUI.Label(position, label);
            }
            
            if (GUI.Button(buttonRect, "Edit"))
            {
                GraphSpawnMapEditor window = EditorWindow.GetWindow<GraphSpawnMapEditor>();
                window.GraphSpawnMap = spawnMap;
            }
        }
    }
}
