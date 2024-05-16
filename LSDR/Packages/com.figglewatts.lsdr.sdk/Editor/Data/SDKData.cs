using LSDR.SDK.Data;
using UnityEditor;
using UnityEngine;

namespace LSDR.SDK.Editor.Data
{
    public static class SDKData
    {
        public static string GetRecentModOutputPath(LSDRevampedMod mod)
        {
            var key = $"{getModDataKey(mod)}_outputPath";
            if (!EditorPrefs.HasKey(key))
            {
                return string.Empty;
            }
            return EditorPrefs.GetString(key);
        }

        public static void SetRecentModOutputPath(LSDRevampedMod mod, string path)
        {
            var key = $"{getModDataKey(mod)}_outputPath";
            EditorPrefs.SetString(key, path);
        }

        private static string getModDataKey(LSDRevampedMod mod)
        {
            return mod.Name;
        }
    }
}
