using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LSDR.Util
{
    public static class MenuTools
    {
        [MenuItem("LSDR SDK/Open game save path")]
        public static void OpenGameSavePath()
        {
            Process.Start(Application.persistentDataPath);
        }
    }
}
