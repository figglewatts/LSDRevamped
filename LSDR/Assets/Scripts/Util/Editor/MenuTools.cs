using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using LSDR.SDK.Entities;
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

        [MenuItem("GameObject/Increment entity name", false, 0)]
        public static void IncrementEntityName()
        {
            GameObject selectedGameObject = Selection.activeGameObject;
            if (selectedGameObject == null) return;

            void doRename(BaseEntity baseEntity)
            {
                var originalName = baseEntity.ID;

                // if it's ONLY digits, skip renaming (we'd garble the TOD animations..)
                if (Regex.IsMatch(originalName, @"^\d+$")) return;

                bool replaced = false;
                var incrementedName = Regex.Replace(originalName, @"\d+", m =>
                {
                    if (replaced) return m.Value;

                    replaced = true;
                    return (int.Parse(m.Value) + 1).ToString();
                });

                baseEntity.ID = incrementedName;
                baseEntity.OnValidate();
            }

            Transform parentTransform = selectedGameObject.transform;
            foreach (var entity in selectedGameObject.GetComponentsInChildren<BaseEntity>())
            {
                doRename(entity);
            }
        }
    }
}
