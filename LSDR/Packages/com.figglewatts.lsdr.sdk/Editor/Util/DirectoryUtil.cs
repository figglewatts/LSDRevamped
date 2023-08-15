using UnityEngine;

namespace LSDR.SDK.Editor.Util
{
    public static class DirectoryUtil
    {
        public static string MakePathAssetsRelative(string absolutePath)
        {
            if (string.IsNullOrEmpty(absolutePath))
            {
                return string.Empty;
            }

            // Ensure the path uses forward slashes, as Unity does.
            string formattedAbsolutePath = absolutePath.Replace("\\", "/");

            if (formattedAbsolutePath.StartsWith(Application.dataPath))
            {
                // Subtract the full project path to get the relative path.
                return "Assets" + formattedAbsolutePath.Substring(Application.dataPath.Length);
            }
            else
            {
                Debug.LogWarning("The provided path is not within the Unity project: " + absolutePath);
                return null;
            }
        }
    }
}
