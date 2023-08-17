using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Torii.Util
{
    // adapted from: https://answers.unity.com/questions/960413/editor-window-how-to-center-a-window.html
    public static class EditorWindowExtensions
    {
        public static Type[] GetAllDerivedTypes(this AppDomain aAppDomain, Type aType)
        {
            var result = new List<Type>();
            Assembly[] assemblies = aAppDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    if (type.IsSubclassOf(aType))
                        result.Add(type);
                }
            }

            return result.ToArray();
        }

        public static Rect GetEditorMainWindowPos()
        {
            Type containerWinType = AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(ScriptableObject))
                                             .FirstOrDefault(t => t.Name == "ContainerWindow");
            if (containerWinType == null)
            {
                throw new MissingMemberException(
                    "Can't find internal type ContainerWindow. Maybe something has changed inside Unity");
            }
            FieldInfo showModeField = containerWinType.GetField("m_ShowMode",
                BindingFlags.NonPublic | BindingFlags.Instance);
            PropertyInfo positionProperty = containerWinType.GetProperty("position",
                BindingFlags.Public | BindingFlags.Instance);
            if (showModeField == null || positionProperty == null)
            {
                throw new MissingFieldException(
                    "Can't find internal fields 'm_ShowMode' or 'position'. Maybe something has changed inside Unity");
            }
            Object[] windows = Resources.FindObjectsOfTypeAll(containerWinType);
            foreach (Object win in windows)
            {
                int showmode = (int)showModeField.GetValue(win);
                if (showmode == 4) // main window
                {
                    Rect pos = (Rect)positionProperty.GetValue(win, index: null);
                    return pos;
                }
            }

            throw new NotSupportedException(
                "Can't find internal main window. Maybe something has changed inside Unity");
        }

        public static void CenterOnMainWindow(this EditorWindow aWin)
        {
            Rect main = GetEditorMainWindowPos();
            Rect pos = aWin.position;
            float w = (main.width - pos.width) * 0.5f;
            float h = (main.height - pos.height) * 0.5f;
            pos.x = main.x + w;
            pos.y = main.y + h;
            aWin.position = pos;
        }
    }
}
