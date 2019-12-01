using System;
using UnityEditor;
using UnityEngine;

namespace LSDR.SDK
{
    public static class CommonGUI
    {
        public static string BrowseForFile(string dialogTitle, string[] filters, string existingValue)
        {
            GUI.FocusControl(null);
            string filePath = EditorUtility.OpenFilePanelWithFilters(dialogTitle, "", filters);

            if (string.IsNullOrEmpty(filePath))
            {
                return existingValue;
            }

            if (!filePath.Contains("StreamingAssets"))
            {
                EditorUtility.DisplayDialog("File error", "Your file must be in the 'StreamingAssets' directory!",
                    "Ok");
                return existingValue;
            }
        
            // now remove everything before StreamingAssets path
            var indexOf = filePath.IndexOf("StreamingAssets", StringComparison.Ordinal) + "StreamingAssets".Length;
            return filePath.Substring(indexOf);
        }

        public static string BrowseForFolder(string dialogTitle, string[] filters, string existingValue)
        {
            GUI.FocusControl(null);
            string folderPath = EditorUtility.OpenFolderPanel(dialogTitle, "", "");

            if (string.IsNullOrEmpty(folderPath))
            {
                return existingValue;
            }
            
            if (!folderPath.Contains("StreamingAssets"))
            {
                EditorUtility.DisplayDialog("Directory error", "Your directory must be in the 'StreamingAssets' directory!",
                    "Ok");
                return existingValue;
            }
        
            // now remove everything before StreamingAssets path
            var indexOf = folderPath.IndexOf("StreamingAssets", StringComparison.Ordinal) + "StreamingAssets".Length;
            return folderPath.Substring(indexOf);
        }
    }
}
