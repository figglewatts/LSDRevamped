using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LSDR.SDK.Editor.Util
{
    public static class EditorWindowExtensions
    {
        private static Type GetType(this AppDomain aAppDomain, Type aType, string name)
        {
            Assembly[] assemblies = aAppDomain.GetAssemblies();
            return assemblies.SelectMany(assembly => assembly.GetTypes())
                             .FirstOrDefault(type => type.IsSubclassOf(aType) && type.Name == name);
        }

        public static Rect GetEditorMainWindowPos()
        {
            Type containerWinType = AppDomain.CurrentDomain.GetType(typeof(ScriptableObject), "ContainerWindow");
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
