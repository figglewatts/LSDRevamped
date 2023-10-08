using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace LSDR.Util
{
    public static class MenuTools
    {
        [MenuItem("LSDR SDK/Open game save path")]
        public static void OpenGameSavePath()
        {
            Process.Start(Application.persistentDataPath);
        }

        [MenuItem("GameObject/Reorder LBDs", false, 0)]
        public static void ReorderChildrenByName()
        {
            GameObject selectedGameObject = Selection.activeGameObject;
            if (selectedGameObject != null)
            {
                Transform parentTransform = selectedGameObject.transform;
                var children = new List<Transform>();
                foreach (var child in parentTransform)
                {
                    children.Add((Transform)child);
                }

                var orderedChildren = children.OrderBy(t =>
                {
                    Debug.Log(t.name);
                    return int.Parse(t.name.Substring(1));
                }).ToArray();

                for (int i = 0; i < orderedChildren.Length; i++)
                {
                    orderedChildren[i].SetSiblingIndex(i);
                }
            }
        }
    }
}
